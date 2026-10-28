using System.Globalization;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;


internal sealed class IntegerFilterValueSearchFilterDecorator<TProjection> : FilterValueSearchFilterDecorator<TProjection>
{
    public IntegerFilterValueSearchFilterDecorator(ISearchFilter<TProjection> inner) : base(inner)
    {
    }

    protected override object Convert(object value)
    {
        return value switch
        {
            int intValue =>
                intValue,

            string stringValue when int.TryParse(
                stringValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int result) =>
                    result,

            _ => value
        };
    }
}
