using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;

internal sealed class EstablishmentBuilder
{
    private readonly Establishment _dto;

    public EstablishmentBuilder()
    {
        _dto = new Establishment
        {
            Urn = "123456",
            Name = "Test School",
            EstablishmentNumber = "123",

            EstablishmentStatus = new EstablishmentStatus
            {
                Name = "Open"
            },

            EstablishmentType = new EstablishmentType
            {
                Name = "Academy"
            },

            EstablishmentProvision = new EstablishmentProvision
            {
                EducationPhase = new EducationPhase
                {
                    Name = "Primary"
                }
            },

            EstablishmentAdmissions = new EstablishmentAdmissions
            {
                StatutoryLowAge = 5,
                StatutoryHighAge = 11
            },

            EstablishmentLifecycleEvent = new List<EstablishmentLifecycleEvent>
            {
                new EstablishmentLifecycleEvent
                {
                    EventType = "Opened",
                    EventDate = new DateOnly(2000, 1, 1),
                    OpenedReason = new ReasonEstablishmentOpened
                    {
                        Name = "New School"
                    }
                },
                new EstablishmentLifecycleEvent
                {
                    EventType = "Closed",
                    EventDate = new DateOnly(2020, 1, 1),
                    ClosedReason = new ReasonEstablishmentClosed
                    {
                        Name = "Merged"
                    }
                }
            },

            Site = new List<Site>
            {
                new Site
                {
                    Name = "Main Site",
                    AddressLine1 = "1 Test Street",
                    AddressLine2 = "Test Area",
                    Town = "Test Town",
                    County = "Test County",
                    Postcode = "TE1 1ST"
                }
            }
        };
    }

    public EstablishmentBuilder WithUrn(string urn)
    {
        _dto.Urn = urn;
        return this;
    }

    public EstablishmentBuilder WithName(string name)
    {
        _dto.Name = name;
        return this;
    }

    public EstablishmentBuilder WithNumber(string number)
    {
        _dto.EstablishmentNumber = number;
        return this;
    }

    public Establishment Build() => _dto;
}


internal static class EstablishmentFactory
{
    public static Establishment Create()
    {
        return CreateMany(1).Single();
    }

    public static IReadOnlyCollection<Establishment> CreateMany(int count)
    {
        List<Establishment> list = new List<Establishment>(count);

        for (int i = 0; i < count; i++)
        {
            list.Add(
                new EstablishmentBuilder()
                    .WithUrn((100000 + i).ToString())
                    .Build());
        }

        return list.AsReadOnly();
    }
}
