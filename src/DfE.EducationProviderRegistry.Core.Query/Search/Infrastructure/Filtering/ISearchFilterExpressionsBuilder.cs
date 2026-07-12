namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

public interface ISearchFilterExpressionsBuilder
{
    /// <summary>
    /// Allows the construction of an OData filter expression(s) composition in string format.
    /// </summary>
    /// <param name="searchFilterRequests">
    /// The collection of <see cref="SearchFilterRequest"/> types which are used to reconcile
    /// to the underlying OData filter expressions.
    /// </param>
    /// <returns>
    /// A string which represents the formatted OData filter expression(s) composition.
    /// </returns>
    string BuildSearchFilterExpressions(IEnumerable<SearchFilterRequest> searchFilterRequests);
}
