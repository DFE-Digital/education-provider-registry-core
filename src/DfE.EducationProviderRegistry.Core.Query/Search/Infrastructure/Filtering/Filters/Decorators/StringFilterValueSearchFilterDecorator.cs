using System.Globalization;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.FilterValueConverters;

internal sealed class StringFilterValueSearchFilterDecorator<TProjection> : FilterValueSearchFilterDecorator<TProjection>
{
    public StringFilterValueSearchFilterDecorator(ISearchFilter<TProjection> inner) : base(inner)
    {
    }

    protected override object Convert(object value)
    {
        return value switch
        {
            string stringValue => stringValue,

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
