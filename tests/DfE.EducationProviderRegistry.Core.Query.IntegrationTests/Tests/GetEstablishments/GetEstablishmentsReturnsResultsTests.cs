using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        IReadOnlyCollection<EstablishmentAggregate> seed =
            EstablishmentAggregateTestDoubles.StubMany(count: 10);

        EstablishmentAggregates estab = await DatabaseFixture.SeedAsync<
            IEnumerable<EstablishmentAggregate>, EstablishmentAggregates>(
                seed, TestContext.Current.CancellationToken);

        GetEstablishmentsRequest request = GetEstablishmentsRequest.Create();

        // act
        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> results =
            await ExecuteUseCase<
                GetEstablishmentsRequest, IReadOnlyCollection<EstablishmentDetailsModel>>(request, TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(results);
        Assert.Equal(seed.Count, results.Model.Count);
    }
}
