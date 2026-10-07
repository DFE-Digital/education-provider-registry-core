using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentIdentifiersModel
{
    private readonly EstablishmentUrnModel _urn;
    private readonly Ukprn? _ukprn;
    private readonly DfeNumber? _dfeNumber;

    public EstablishmentIdentifiersModel(EstablishmentUrnModel urn, Ukprn? ukprn, DfeNumber? dfeNumber)
    {
        ArgumentNullException.ThrowIfNull(urn);
        _urn = urn;
        _ukprn = ukprn;
        _dfeNumber = dfeNumber;
    }

    public string Urn => _urn.Value;
    public string? Ukprn => _ukprn?.Value;
    public string? DfENumber => _dfeNumber?.Value;
    public string? LaEstab => _dfeNumber?.Value.Remove('/');
}
