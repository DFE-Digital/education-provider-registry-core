using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Query.Service.Persistence.Establishments.DataTransferObjects;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Query.Service.Persistence.Establishments;

public sealed class EstablishmentsRepository
{
    private readonly IMapper<
        IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>> _establishmentsMapper;

    EstablishmentsRepository(
        IMapper<
            IEnumerable<EstablishmentDataTransferObject>,
            IReadOnlyCollection<Establishment>> establishmentsMapper)
    {
        _establishmentsMapper = establishmentsMapper;
    }

    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        CancellationToken cancellationToken = default)
    {
        // TEMPORARY: Fake data until SQL is wired up
        IEnumerable<EstablishmentDataTransferObject> dtos =
            FakeEstablishmentDataGenerator.Generate(100);

        return _establishmentsMapper.Map(dtos);
    }

    internal static class FakeEstablishmentDataGenerator
    {
        public static IReadOnlyCollection<EstablishmentDataTransferObject> Generate(int count)
        {
            HashSet<string> urns = GenerateUniqueUrns(count);
            List<EstablishmentDataTransferObject> dtos = [];

            foreach (string urn in urns)
            {
                EstablishmentDataTransferObject dto = new()
                {
                    URN = urn
                };

                dtos.Add(dto);
            }

            return dtos.AsReadOnly();
        }

        private static HashSet<string> GenerateUniqueUrns(int count)
        {
            HashSet<string> urns = [];
            Random random = new();

            while (urns.Count < count)
            {
                int number = random.Next(100000, 999999); // 6 digits
                string urn = number.ToString();
                urns.Add(urn);
            }

            return urns;
        }
    }
}
