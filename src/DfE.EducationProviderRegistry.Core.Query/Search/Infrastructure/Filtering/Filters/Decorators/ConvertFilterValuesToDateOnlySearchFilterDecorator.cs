using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;

internal sealed class ConvertFilterValuesToDateOnlySearchFilterDecorator<TProjection> : FilterValueSearchFilterDecorator<TProjection>
{
    public ConvertFilterValuesToDateOnlySearchFilterDecorator(ISearchFilter<TProjection> inner) : base(inner)
    {
    }

#nullable disable
    protected override object Convert(object value)
    {
        return value switch
        {
            null => null,

            DateOnly dateOnly =>
                dateOnly,

            DateTime dateTime =>
                DateOnly.FromDateTime(dateTime),

            string stringValue when DateOnly.TryParseExact(
                stringValue,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly result) =>
                    result,

            _ => value
        };
    }
#nullable enable    
}
