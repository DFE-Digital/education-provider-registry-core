namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

public sealed class SearchTerm
{
    public SearchTerm(string key, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key.Trim();
        // Absence of value not required currently - normalise
        Value = value?.Trim() ?? string.Empty;
    }

    public string Key { get; }
    public string Value { get; }
}
