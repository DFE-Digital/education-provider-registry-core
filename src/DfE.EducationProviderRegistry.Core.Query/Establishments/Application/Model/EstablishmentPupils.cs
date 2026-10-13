namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public readonly record struct EstablishmentPupils
{
    public EstablishmentPupils(int total)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        Total = total;
    }

    public int Total { get; }
}
