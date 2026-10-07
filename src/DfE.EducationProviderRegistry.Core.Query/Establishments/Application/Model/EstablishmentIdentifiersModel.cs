namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentIdentifiersModel
{
    private readonly EstablishmentUrnModel _urn;
    private readonly DfeNumber? _dfeNumber;

    public EstablishmentIdentifiersModel(EstablishmentUrnModel urn, DfeNumber? dfeNumber)
    {
        ArgumentNullException.ThrowIfNull(urn);
        _urn = urn;
        _dfeNumber = dfeNumber;
    }

    public string Urn => _urn.Value;
    // TODO normalise ABC abc?
    public string? DfENumber => _dfeNumber?.Value;
    public string? LaEstab => _dfeNumber?.Value.Replace("/", string.Empty);
}
