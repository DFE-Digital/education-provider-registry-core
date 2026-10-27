namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SearchOpenedDateFilter : SearchDateRangeFilter
{
    public SearchOpenedDateFilter()
        : base(searchAggregate => searchAggregate.OpenedDate)
    {
    }
}

