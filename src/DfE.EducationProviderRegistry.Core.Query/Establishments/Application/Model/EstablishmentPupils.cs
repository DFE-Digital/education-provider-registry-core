namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public readonly record struct EstablishmentPupils
{
    public EstablishmentPupils(int total, int boys, int girls)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        ArgumentOutOfRangeException.ThrowIfNegative(boys);
        ArgumentOutOfRangeException.ThrowIfNegative(girls);
        Total = total;
        Boys = boys;
        Girls = girls;
    }

    public int Total { get; }
    public int Boys { get; }
    public int Girls { get; }
}
