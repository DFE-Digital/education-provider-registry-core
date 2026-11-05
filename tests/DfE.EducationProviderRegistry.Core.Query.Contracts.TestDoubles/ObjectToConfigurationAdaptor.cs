using System.Collections;
using System.Globalization;
using System.Reflection;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles;

public static class ObjectToConfigurationAdaptor
{
    public static IReadOnlyDictionary<string, string?> ToConfiguration<T>(T instance, string rootConfigurationKey)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentException.ThrowIfNullOrWhiteSpace(rootConfigurationKey);

        Dictionary<string, string?> configuration = [];

        AddValue(
            configuration,
            rootConfigurationKey,
            instance);

        return configuration;
    }

    private static void AddValue(
        IDictionary<string, string?> configuration,
        string configurationKey,
        object? value)
    {
        if (value is null)
        {
            configuration.Add(configurationKey, null);
            return;
        }

        Type valueType = value.GetType();

        if (IsScalar(valueType))
        {
            configuration.Add(
                configurationKey,
                ConvertScalarToString(value));

            return;
        }

        if (value is IDictionary dictionary)
        {
            AddDictionary(
                configuration,
                configurationKey,
                dictionary);

            return;
        }

        if (value is IEnumerable enumerable)
        {
            AddEnumerable(
                configuration,
                configurationKey,
                enumerable);

            return;
        }

        AddObject(
            configuration,
            configurationKey,
            value,
            valueType);
    }

    private static void AddDictionary(
        IDictionary<string, string?> configuration,
        string configurationKey,
        IDictionary dictionary)
    {
        foreach (DictionaryEntry entry in dictionary)
        {
            if (entry.Key is null)
            {
                throw new InvalidOperationException(
                    $"The dictionary at '{configurationKey}' contains a null key.");
            }

            string dictionaryKey =
                Convert.ToString(entry.Key, CultureInfo.InvariantCulture)
                ?? throw new InvalidOperationException(
                    $"The dictionary key at '{configurationKey}' could not be converted to a string.");

            AddValue(
                configuration,
                $"{configurationKey}:{dictionaryKey}",
                entry.Value);
        }
    }

    private static void AddEnumerable(
        IDictionary<string, string?> configuration,
        string configurationKey,
        IEnumerable enumerable)
    {
        int index = 0;

        foreach (object? item in enumerable)
        {
            AddValue(
                configuration,
                $"{configurationKey}:{index}",
                item);

            index++;
        }
    }

    private static void AddObject(
        IDictionary<string, string?> configuration,
        string configurationKey,
        object value,
        Type valueType)
    {
        PropertyInfo[] properties = valueType.GetProperties(
            BindingFlags.Instance | BindingFlags.Public);

        foreach (PropertyInfo property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            if (property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            object? propertyValue = property.GetValue(value);

            AddValue(
                configuration,
                $"{configurationKey}:{property.Name}",
                propertyValue);
        }
    }

    private static bool IsScalar(Type type)
    {
        Type scalarType = Nullable.GetUnderlyingType(type) ?? type;

        return scalarType.IsPrimitive
            || scalarType.IsEnum
            || scalarType == typeof(string)
            || scalarType == typeof(decimal)
            || scalarType == typeof(DateOnly)
            || scalarType == typeof(TimeOnly)
            || scalarType == typeof(DateTime)
            || scalarType == typeof(DateTimeOffset)
            || scalarType == typeof(TimeSpan)
            || scalarType == typeof(Guid)
            || scalarType == typeof(Uri);
    }

    private static string? ConvertScalarToString(object value)
    {
        return value switch
        {
            DateOnly dateOnly =>
                dateOnly.ToString("O", CultureInfo.InvariantCulture),

            TimeOnly timeOnly =>
                timeOnly.ToString("O", CultureInfo.InvariantCulture),

            DateTime dateTime =>
                dateTime.ToString("O", CultureInfo.InvariantCulture),

            DateTimeOffset dateTimeOffset =>
                dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),

            TimeSpan timeSpan =>
                timeSpan.ToString("c", CultureInfo.InvariantCulture),

            IFormattable formattable =>
                formattable.ToString(null, CultureInfo.InvariantCulture),

            _ => value.ToString()
        };
    }
}
