using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments.Extensions;
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
            .AddGetEstablishments();
    }

    [Fact]
    public async Task Returns_Results()
    {
        // arrange
        GetEstablishmentsRequest request = GetEstablishmentsRequest.Create();

        // act
        UseCaseResponse<IReadOnlyCollection<EstablishmentDetailsModel>> results =
            await ExecuteUseCase<
                GetEstablishmentsRequest, IReadOnlyCollection<EstablishmentDetailsModel>>(request);

        // assert
        Assert.NotNull(results);

        Assert.NotNull(results.Model);
        Assert.NotEmpty(results.Model);
    }
}
