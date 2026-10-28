using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;

internal abstract class FilterValueSearchFilterDecorator<TProjection> : ISearchFilter<TProjection>
{
    private readonly ISearchFilter<TProjection> _inner;

    protected FilterValueSearchFilterDecorator(
        ISearchFilter<TProjection> inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
    }

    public ISpecification<TProjection> CreateSpecification(
        SearchFilterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        object[] values = [.. request.FilterValues.Select(Convert)];

        SearchFilterRequest convertedRequest = new(request.FilterKey, values);

        if (!string.IsNullOrWhiteSpace(request.FilterValuesDelimiter))
        {
            convertedRequest.SetFilterValuesDelimiter(
                request.FilterValuesDelimiter);
        }

        return _inner.CreateSpecification(convertedRequest);
    }

    protected abstract object Convert(object value);
}
