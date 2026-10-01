using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;

public sealed class EstablishmentDetailsBuilder
{
    private static int _urnCounter = 100_500;
    private int? _urn;

    public EstablishmentDetailsBuilder WithUrn(int urn)
    {
        _urn = urn;
        return this;
    }

    public EstablishmentDetailsModel Build()
    {
        string urn =
            (_urn is not null ?
                _urn.Value : Interlocked.Increment(ref _urnCounter)).ToString();

        return new()
        {
            Urn = new EstablishmentUrnModel(
                new UniqueReferenceNumber(urn))
        };
    }

    public static EstablishmentDetailsBuilder Create() => new();
}
