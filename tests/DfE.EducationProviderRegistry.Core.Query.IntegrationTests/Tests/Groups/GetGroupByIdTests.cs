using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Group;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Groups;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Groups;

public sealed class GetGroupByIdTests : UseCaseIntegrationTestBase
{
    public GetGroupByIdTests(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGetGroups(configuration);
    }

    [Fact]
    public async Task Returns_MappedGroup()
    {
        // Arrange
        GroupAggregate group = new GroupAggregateBuilder()
            .WithGroupUid(12345)
            .WithName("Test Trust")
            .WithGroupId("TR001")
            .WithCompaniesHouseNumber("COMP123")
            .WithUkprn("10000001")
            .WithGroupType("MAT")
            .Build();

        await DatabaseFixture.SeedAsync<
            IEnumerable<GroupAggregate>,
            GroupAggregates>([group], TestContext.Current.CancellationToken);

        GetGroupByGroupUniqueIdentifierRequest request = new(GroupUid: "12345");

        // Act
        UseCaseResponse<GroupReadModelResponse> result =
            await ExecuteUseCase<GetGroupByGroupUniqueIdentifierRequest, GroupReadModelResponse>(
                request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Model.Group);

        GroupReadModel actual = result.Model.Group;

        Assert.Equal(12345, actual.GroupUID);
        Assert.Equal("Test Trust", actual.Name);
        Assert.Equal("TR001", actual.GroupId);
        Assert.Equal("COMP123", actual.CompaniesHouseId);
        Assert.Equal("10000001", actual.UKPRN);
        Assert.Equal("MAT", actual.Type);
    }
}
