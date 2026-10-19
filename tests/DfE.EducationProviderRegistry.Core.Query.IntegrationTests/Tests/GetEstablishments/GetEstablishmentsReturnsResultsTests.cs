using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Noding;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments;

public sealed class GetEstablishmentsReturnsResultsTests : UseCaseIntegrationTestBase
{
    public GetEstablishmentsReturnsResultsTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddGetEstablishments(configuration);
    }

    [Fact]
    public async Task Returns_Results()
    {
        // Arrange
        const int seedCount = 10;
        IReadOnlyList<EstablishmentAggregate> seed =
            [.. EstablishmentAggregateTestDoubles.StubMany(count: seedCount)];

        await DatabaseFixture.SeedAsync<
            IEnumerable<EstablishmentAggregate>, EstablishmentAggregates>(
                seed, TestContext.Current.CancellationToken);

        GetEstablishmentsRequest request = GetEstablishmentsRequest.Create();

        // act
        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> results =
            await ExecuteUseCase<
                GetEstablishmentsRequest, IReadOnlyCollection<EstablishmentDetailsModel>>(request, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(results);
        Assert.Equal(seedCount, results.Model.Count);
    }

    [Fact]
    public async Task Returns_MappedEstablishment()
    {
        // Arrange
        EstablishmentAggregate establishment =
            EstablishmentAggregateBuilder.Create()
                .WithUrn("123456")
                .WithName("Test School")
                .WithGroupName("Trust A")
                .WithGroupCode("TRST")
                .WithWebsite("www.test.com")
                .Build();

        await DatabaseFixture.SeedAsync<IEnumerable<EstablishmentAggregate>, EstablishmentAggregates>([establishment], TestContext.Current.CancellationToken);

        GetEstablishmentsRequest request =
            GetEstablishmentsRequest.Create();

        // Act
        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> result =
            await ExecuteUseCase<
                GetEstablishmentsRequest,
                IReadOnlyCollection<EstablishmentDetailsModel>>(
                    request,
                    TestContext.Current.CancellationToken);

        EstablishmentDetailsModel school =
            Assert.Single(result.Model);

        // Assert
        Assert.Equal("123456", school.Identifiers.Urn);
        Assert.Equal("Test School", school.Name.Value);

        Assert.NotNull(school.Group);
        Assert.Equal("Trust A", school.Group.name);
        Assert.Equal("TRST", school.Group.code);

        Assert.NotNull(school.ContactDetails);
        Assert.Equal("www.test.com", school.ContactDetails.Website);
    }
}
