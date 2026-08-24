namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public static class PropertyPathParser
{
    public static ParsedPropertyPath Parse(string fieldPath)
    {
        ArgumentNullException.ThrowIfNull(fieldPath);

        bool isCollection = fieldPath.Contains("[]", StringComparison.Ordinal);

        if (!isCollection)
        {
            return new ParsedPropertyPath(
                false,
                fieldPath,
                string.Empty
            );
        }

        int index = fieldPath.IndexOf("[]", StringComparison.Ordinal);

        return new ParsedPropertyPath(
            true,
            fieldPath[..index],
            fieldPath[(index + 3)..]
        );
    }
}
