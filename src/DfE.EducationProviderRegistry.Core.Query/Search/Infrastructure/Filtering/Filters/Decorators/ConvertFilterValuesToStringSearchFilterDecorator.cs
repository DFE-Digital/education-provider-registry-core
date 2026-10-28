using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;

internal sealed class ConvertFilterValuesToStringSearchFilterDecorator<TProjection> : FilterValueSearchFilterDecorator<TProjection>
{
    public ConvertFilterValuesToStringSearchFilterDecorator(ISearchFilter<TProjection> inner) : base(inner)
    {
    }

    protected override object Convert(object value)
    {
        return value switch
        {
            string stringValue =>
                stringValue,

            byte or sbyte or
            short or ushort or
            int or uint or
            long or ulong or
            float or double or decimal =>
                ((IFormattable)value)
                    .ToString(null, CultureInfo.InvariantCulture),

            _ => value
        };
    }
}
