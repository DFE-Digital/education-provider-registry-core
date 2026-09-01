using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps;

public sealed class ParallelMappingStepUnitTests
{
    [Fact]
    public async Task HandleAsync_Throws_WhenEstablishmentsMissing()
    {
        // arrange
        Mock<IMapper<Establishment, EstablishmentSearchResult>> mapperMock = new();

        ParallelMappingStep step = new(mapperMock.Object);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                establishments: null);

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains(
            "PipelineContext does not contain a value of type",
            ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCancellationRequested()
    {
        // arrange
        Mock<IMapper<Establishment, EstablishmentSearchResult>> mapperMock =
            EstablishmentToSearchResultMapperTestDouble.MockFor(
                new EstablishmentSearchResult(
                    new UniqueReferenceNumber("00001"),
                    new Name("School A"),
                    new SiteAddressModel(
                        Name: string.Empty,
                        AddressLine1: "123 Street",
                        AddressLine2: string.Empty,
                        Town: "Town",
                        County: "County",
                        Postcode: "AA1 1AA"
                    ),
                    new EstablishmentType("Academy"),
                    new GroupDetail("Group", "G"),
                    new LocalAuthority("LA", "Authority")));

        List<Establishment> establishments =
        [
            new Establishment { Urn = "00001" }
        ];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(establishments);

        ParallelMappingStep step = new(mapperMock.Object);

        using CancellationTokenSource cts = new();

        cts.Cancel();

        // act // assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => step.HandleAsync(
                context,
                cts.Token).AsTask());
    }

    [Fact]
    public async Task HandleAsync_MapsAllEstablishmentsInParallel_AndPreservesOrdering()
    {
        // arrange
        Mock<IMapper<Establishment, EstablishmentSearchResult>> mapperMock =
            EstablishmentToSearchResultMapperTestDouble.MockFor(
                (Establishment establishment) =>
                    new EstablishmentSearchResult(
                        new UniqueReferenceNumber(establishment.Urn!),
                        new Name($"Mapped {establishment.Urn}"),
                        new SiteAddressModel(
                            Name: string.Empty,
                            AddressLine1: "123 Street",
                            AddressLine2: string.Empty,
                            Town: "Town",
                            County: "County",
                            Postcode: "AA1 1AA"
                        ),
                        new EstablishmentType("Academy"),
                        new GroupDetail("Group", "G"),
                        new LocalAuthority("LA", "Authority")));

        List<Establishment> establishments =
        [
            new Establishment { Urn = "00001" },
            new Establishment { Urn = "00002" },
            new Establishment { Urn = "00003" }
        ];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(establishments);

        ParallelMappingStep step = new(mapperMock.Object);

        // act
        await step.HandleAsync(
            context,
            TestContext.Current.CancellationToken);

        // assert
        EstablishmentSearchResult[] results =
            context.Get<EstablishmentSearchResult[]>();

        Assert.Equal(3, results.Length);

        Assert.Equal("00001", results[0].Urn.Value);
        Assert.Equal("00002", results[1].Urn.Value);
        Assert.Equal("00003", results[2].Urn.Value);

        Assert.Equal("Mapped 00001", results[0].Name.Value);
        Assert.Equal("Mapped 00002", results[1].Name.Value);
        Assert.Equal("Mapped 00003", results[2].Name.Value);
    }
}
