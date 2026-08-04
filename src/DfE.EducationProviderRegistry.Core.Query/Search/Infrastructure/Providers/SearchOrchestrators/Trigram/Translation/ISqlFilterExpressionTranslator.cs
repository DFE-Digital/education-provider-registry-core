using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;

/// <summary>
/// Defines a component capable of translating a LINQ predicate expression into
/// a SQL filter fragment suitable for trigram‑based search queries.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the expression operates on.
/// </typeparam>
public interface ISqlFilterExpressionTranslator<TProjection>
{
    /// <summary>
    /// Translates the supplied predicate expression into a SQL filter fragment,
    /// using the resolved <see cref="EntityMetadata"/> for column mapping.
    /// </summary>
    /// <param name="expression">
    /// The predicate expression to translate.
    /// </param>
    /// <param name="metadata">
    /// The resolved EF Core metadata for <typeparamref name="TProjection"/>,
    /// including table, schema, and primary key information.
    /// </param>
    /// <returns>
    /// A SQL fragment representing the translated filter expression.
    /// </returns>
    string Translate(Expression<Func<TProjection, bool>> expression,
                     EntityMetadata metadata);
}
