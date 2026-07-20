using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Test;


public enum SearchMode
{
    Numeric,
    Text
}

public sealed record SearchQueryDefinition(
    IReadOnlyCollection<string> WhereClauses,
    IReadOnlyCollection<string> OrderByClauses);

public interface ISearchClauseBuilder
{
    string BuildExactMatch(
        IReadOnlyCollection<string> columns,
        string searchTerm);

    string BuildPartialMatch(
        IReadOnlyCollection<string> columns,
        string searchTerm,
        bool prefixOnly);

    string BuildFuzzyMatch(
        IReadOnlyCollection<string> columns,
        string searchTerm);

    string BuildSimilarity(
        IReadOnlyCollection<string> columns,
        string searchTerm);
}

public sealed class SearchClauseBuilder : ISearchClauseBuilder
{
    public string BuildExactMatch(
        IReadOnlyCollection<string> columns,
        string searchTerm)
    {
        return string.Join(
            " OR ",
            columns.Select(c =>
                $@"t.""{c}"" = CAST('{searchTerm}' AS text)"));
    }

    public string BuildPartialMatch(
        IReadOnlyCollection<string> columns,
        string searchTerm,
        bool prefixOnly)
    {
        string pattern = prefixOnly
            ? $"CAST('{searchTerm}' AS text) || '%'"
            : $"'%' || CAST('{searchTerm}' AS text) || '%'";

        string op = prefixOnly
            ? "LIKE"
            : "ILIKE";

        return string.Join(
            " OR ",
            columns.Select(c =>
                $@"t.""{c}"" {op} {pattern}"));
    }

    public string BuildFuzzyMatch(
        IReadOnlyCollection<string> columns,
        string searchTerm)
    {
        return string.Join(
            " OR ",
            columns.Select(c =>
                $@"t.""{c}"" % CAST('{searchTerm}' AS text)"));
    }

    public string BuildSimilarity(
        IReadOnlyCollection<string> columns,
        string searchTerm)
    {
        return string.Join(
            ", ",
            columns.Select(c =>
                $@"similarity(t.""{c}"", CAST('{searchTerm}' AS text))"));
    }
}


public interface ISearchQueryDefinitionFactory
{
    SearchQueryDefinition Create(
        SearchMode mode,
        SearchOrchestratorContext context);
}

public sealed class SearchQueryDefinitionFactory
    : ISearchQueryDefinitionFactory
{
    private readonly ISearchClauseBuilder _clauseBuilder;

    public SearchQueryDefinitionFactory(
        ISearchClauseBuilder clauseBuilder)
    {
        _clauseBuilder = clauseBuilder;
    }

    public SearchQueryDefinition Create(
        SearchMode mode,
        SearchOrchestratorContext context)
    {
        string exactClause =
            _clauseBuilder.BuildExactMatch(
                context.SearchColumns,
                context.SearchTerm);

        string partialClause =
            _clauseBuilder.BuildPartialMatch(
                context.SearchColumns,
                context.SearchTerm,
                prefixOnly: mode == SearchMode.Numeric);

        if (mode == SearchMode.Numeric)
        {
            return new SearchQueryDefinition(
                [exactClause, partialClause],
                [
                    $@"
                    CASE
                        WHEN ({exactClause}) THEN 1
                        ELSE 2
                    END",
                    $@"t.""{context.SearchColumns.First()}"" ASC"
                ]);
        }

        string fuzzyClause =
            _clauseBuilder.BuildFuzzyMatch(
                context.SearchColumns,
                context.SearchTerm);

        string similarityClause =
            _clauseBuilder.BuildSimilarity(
                context.SearchColumns,
                context.SearchTerm);

        return new SearchQueryDefinition(
            [exactClause, partialClause, fuzzyClause],
            [
                $@"
                CASE
                    WHEN ({exactClause}) THEN 1
                    WHEN ({partialClause}) THEN 2
                    ELSE 3
                END",
                $"GREATEST({similarityClause}) DESC"
            ]);
    }
}


public interface ISearchQueryBuilder
{
    string Build(
        EntityMetadata metadata,
        SearchOrchestratorContext context,
        SearchQueryDefinition definition,
        string searchFilters);
}

public sealed class SearchQueryBuilder : ISearchQueryBuilder
{
    public string Build(
        EntityMetadata metadata,
        SearchOrchestratorContext context,
        SearchQueryDefinition definition,
        string searchFilters)
    {
        string whereClause =
            string.Join(
                $"{Environment.NewLine} OR {Environment.NewLine}",
                definition.WhereClauses);

        string orderByClause =
            string.Join(
                ", ",
                definition.OrderByClauses);

        return $@"
            SELECT t.""{metadata.PrimaryKeyColumn}""
            FROM {metadata.Schema}.""{metadata.TableName}"" t
            WHERE (
                {whereClause}
            )
            {searchFilters}
            ORDER BY
                {orderByClause}
            LIMIT {context.PageSize}
            OFFSET {context.Offset}";
    }
}


public interface ISearchColumnValidator
{
    void Validate<TProjection>(
        EntityMetadata metadata,
        SearchOrchestratorContext context);
}

public sealed class SearchColumnValidator : ISearchColumnValidator
{
    public void Validate<TProjection>(
        EntityMetadata metadata,
        SearchOrchestratorContext context)
    {
        List<string> invalidColumns = context.SearchColumns
            .Where(searchColumn =>
                !metadata.EntityType
                    .GetProperties()
                    .Any(property =>
                        property.GetColumnName() == searchColumn))
            .ToList();

        if (invalidColumns.Count > 0)
        {
            throw new InvalidOperationException(
                $"Column(s) '{string.Join("', '", invalidColumns)}' do not exist on entity {typeof(TProjection).Name}.");
        }
    }
}
