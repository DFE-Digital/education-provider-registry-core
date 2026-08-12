using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;

/// <summary>
/// Holds all configuration required for executing a search operation,
/// including search term, paging, target column, and filter expressions.
/// </summary>
/// <typeparam name="TProjection">
/// The projection or entity type being queried.
/// </typeparam>
public sealed record SearchOrchestratorContext<TProjection>
    where TProjection : class
{
    /// <summary>
    /// The raw search term supplied by the caller.
    /// </summary>
    public required string SearchTerm { get; init; }

    /// <summary>
    /// Optional column name to apply the search term against.
    /// </summary>
    public string SearchColumn { get; init; } = string.Empty;

    /// <summary>
    /// Maximum number of records to return.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Number of records to skip before returning results.
    /// </summary>
    public int Offset { get; init; }

    /// <summary>
    /// Filter requests to be applied to the search query.
    /// </summary>
    public IReadOnlyList<SearchFilterRequest> Filters { get; init; } = [];

    /// <summary>
    /// Combined predicate expression built from all filter requests.
    /// </summary>
    public Expression<Func<TProjection, bool>> FilterExpression { get; init; } =
        projection => true;
}
