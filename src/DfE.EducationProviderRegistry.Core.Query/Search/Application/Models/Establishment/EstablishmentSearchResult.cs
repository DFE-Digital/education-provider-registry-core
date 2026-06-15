namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

public class EstablishmentSearchResult
{
    public EstablishmentSearchResult(int urn, string name)
    {
        Urn = urn;
        Name = name;
    }

    public int Urn { get; }
    public string Name { get; }
}
