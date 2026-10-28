using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;

internal sealed class DoubleFilterValueSearchFilterDecorator<TProjection> : FilterValueSearchFilterDecorator<TProjection>
{
    public DoubleFilterValueSearchFilterDecorator(ISearchFilter<TProjection> inner) : base(inner)
    {
    }

    protected override object Convert(object value)
    {
        return value switch
        {
            double doubleValue => doubleValue,

            string stringValue when double.TryParse(
                stringValue,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double result) => result,

            _ => value
        };
    }
}
