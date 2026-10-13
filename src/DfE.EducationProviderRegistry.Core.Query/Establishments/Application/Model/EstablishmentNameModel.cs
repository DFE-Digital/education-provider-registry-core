namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentNameModel
{
    public EstablishmentNameModel(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Value = name.Trim();
    }

    public string Value { get; }
}
