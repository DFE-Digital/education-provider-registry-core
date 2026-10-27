namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SearchClosedDateFilter : SearchDateRangeFilter
{
    public SearchClosedDateFilter()
        : base(searchAggregate => searchAggregate.ClosedDate)
    {
    }
}

