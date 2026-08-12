using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

/// <summary>
/// Builds a composed predicate expression for <typeparamref name="TProjection"/> by
/// resolving filter expressions from incoming <see cref="SearchFilterRequest"/> objects
/// and combining them using a configured logical operator. The resulting expression tree
/// is provider‑agnostic and suitable for translation by the search pipeline.
/// </summary>
/// <typeparam name="TProjection">
/// The projection or entity type the filter expression applies to.
/// </typeparam>
public interface ISearchFilterExpressionsBuilder<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Constructs a composed predicate expression based on the supplied filter requests.
    /// Each request is mapped to a configured filter expression and combined using the
    /// configured logical operator.
    /// </summary>
    /// <param name="searchFilterRequests">
    /// The collection of <see cref="SearchFilterRequest"/> instances describing the
    /// filter keys and values to be applied.
    /// </param>
    /// <returns>
    /// A composed <see cref="Expression{TDelegate}"/> representing the merged predicate
    /// for <typeparamref name="TProjection"/>.
    /// </returns>
    Expression<Func<TProjection, bool>> BuildSearchFilterExpression(
        IEnumerable<SearchFilterRequest> searchFilterRequests);
}
