using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishmentById;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.UseCases.GetEstablishments.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishmentById;

public sealed class GetEstablishmentByIdTests : UseCaseIntegrationTestBase
{
    public GetEstablishmentByIdTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddGetEstablishments(configuration);
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

        GetEstablishmentByIdRequest request = new(Urn: "123456");

        // Act
        UseCaseResponse<EstablishmentDetailsReadModel> result =
            await ExecuteUseCase<
                GetEstablishmentByIdRequest,
                EstablishmentDetailsReadModel>(
                    request,
                    TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.NotNull(result.Model.Establishment);

        EstablishmentDetailsModel resultEstablishment = result.Model.Establishment;

        // Assert
        Assert.Equal("123456", resultEstablishment.Identifiers.Urn);
        Assert.Equal("Test School", resultEstablishment.Name.Value);

        Assert.NotNull(resultEstablishment.Group);
        Assert.Equal("Trust A", resultEstablishment.Group.name);
        Assert.Equal("TRST", resultEstablishment.Group.code);

        Assert.NotNull(resultEstablishment.ContactDetails);
        Assert.Equal("www.test.com", resultEstablishment.ContactDetails.Website);
    }
}
