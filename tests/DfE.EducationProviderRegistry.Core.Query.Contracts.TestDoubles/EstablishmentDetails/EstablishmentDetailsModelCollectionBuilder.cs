using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.EstablishmentDetails;

public sealed class EstablishmentDetailsModelCollectionBuilder
{
    private int _count = 10;

    public EstablishmentDetailsModelCollectionBuilder WithCount(int count = 10)
    {
        _count = count;
        return this;
    }

    public IReadOnlyCollection<EstablishmentDetailsModel> Build()
    {
        List<EstablishmentDetailsModel> establishmentList = new(_count);
        HashSet<int> urns = GenerateUniqueUrns(_count);

        foreach (int urn in urns)
        {
            EstablishmentDetailsModel model =
                EstablishmentDetailsBuilder.Create()
                    .WithUrn(urn)
                    .Build();

            establishmentList.Add(model);
        }

        return establishmentList.AsReadOnly();
    }

    private static HashSet<int> GenerateUniqueUrns(int count)
    {
        HashSet<int> urns = [];
        Random random = new();

        while (urns.Count < count)
        {
            int number = random.Next(100000, 999999); // 6 digits
            urns.Add(number);
        }

        return urns;
    }
}

