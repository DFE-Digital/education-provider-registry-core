using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.FilterValueConverters;

internal sealed class LongFilterValueSearchFilterDecorator<TProjection> : FilterValueSearchFilterDecorator<TProjection>
{
    public LongFilterValueSearchFilterDecorator(ISearchFilter<TProjection> inner) : base(inner)
    {
    }

    protected override object Convert(object value)
    {
        return value switch
        {
            long longValue => longValue,

            string stringValue when long.TryParse(
                stringValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out long result) => result,

            _ => value
        };
    }
}
