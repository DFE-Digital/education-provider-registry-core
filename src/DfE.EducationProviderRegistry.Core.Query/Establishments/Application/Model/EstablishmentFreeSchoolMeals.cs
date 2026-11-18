namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentFreeSchoolMeals
{
    public EstablishmentFreeSchoolMeals(int number, decimal percentage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfNegative(percentage);
        Number = number;
        Percentage = percentage;
    }

    public int Number { get; }
    public decimal Percentage { get; }
}
