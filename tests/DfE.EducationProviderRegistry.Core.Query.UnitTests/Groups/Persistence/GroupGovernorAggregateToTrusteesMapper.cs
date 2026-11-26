using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Persistence;

public sealed class GroupGovernorAggregateToTrusteesMapperTests
{
    [Fact]
    public void Map_GivenNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange
        GroupGovernorAggregateToTrusteesMapper sut = new();

        Func<IReadOnlyCollection<Trustee>> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_GivenEmptyInput_ShouldReturnEmptyCollection()
    {
        // Arrange
        GroupGovernorAggregateToTrusteesMapper sut = new();

        IEnumerable<GroupGovernorAggregate> input = [];

        // Act
        IReadOnlyCollection<Trustee> result = sut.Map(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_GivenMemberGovernor_ShouldExcludeGovernor()
    {
        // Arrange
        GroupGovernorAggregate governor = GroupGovernorAggregateTestDoubles.CreateWith(
            role: "Member",
            roleOrder: 50);

        GroupGovernorAggregateToTrusteesMapper sut = new();

        // Act
        IReadOnlyCollection<Trustee> result = sut.Map([governor]);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_GivenTrusteeGovernor_ShouldMapAllProperties()
    {
        // Arrange
        GroupGovernorAggregate governor = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1548386",
            firstName: "Simon",
            lastName: "Sanger-Anderson",
            role: "Chair of Trustees",
            roleOrder: 30,
            title: "Mr",
            appointingBody: "Appointed by trustees",
            appointmentStartDate: new DateOnly(2016, 12, 5),
            appointmentEndDate: new DateOnly(2026, 8, 27));

        GroupGovernorAggregateToTrusteesMapper sut = new();

        // Act
        IReadOnlyCollection<Trustee> result = sut.Map([governor]);

        // Assert
        Trustee trustee = Assert.Single(result);

        Assert.Equal("1548386", trustee.Id.Value);
        Assert.Equal("Simon Sanger-Anderson", trustee.Name.Value);
        Assert.Equal("Chair of Trustees", trustee.Role);
        Assert.Equal("Mr", trustee.Title);
        Assert.Equal("Appointed by trustees", trustee.AppointedBy);
        Assert.Equal(new DateOnly(2016, 12, 5), trustee.StartDate);
        Assert.Equal(new DateOnly(2026, 8, 27), trustee.EndDate);
    }

    [Fact]
    public void Map_GivenTrusteeWithNoEndDate_ShouldMapNullEndDate()
    {
        // Arrange
        GroupGovernorAggregate governor = GroupGovernorAggregateTestDoubles.CreateWith(
            role: "Trustee",
            roleOrder: 40,
            appointmentEndDate: null);

        GroupGovernorAggregateToTrusteesMapper sut = new();

        // Act
        IReadOnlyCollection<Trustee> result = sut.Map([governor]);

        // Assert
        Trustee trustee = Assert.Single(result);

        Assert.Null(trustee.EndDate);
    }

    [Fact]
    public void Map_GivenMultipleGovernorRoles_ShouldExcludeMembers()
    {
        // Arrange
        GroupGovernorAggregate chair = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000001",
            role: "Chair of Trustees",
            roleOrder: 30);

        GroupGovernorAggregate member = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000002",
            role: "Member",
            roleOrder: 50);

        GroupGovernorAggregate accountingOfficer = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000003",
            role: "Accounting Officer",
            roleOrder: 60);

        GroupGovernorAggregateToTrusteesMapper sut = new();

        // Act
        IReadOnlyCollection<Trustee> result = sut.Map(
            [chair, member, accountingOfficer]);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(
            result,
            trustee => trustee.Id.Value == "1000002");
    }

    [Fact]
    public void Map_GivenMultipleTrustees_ShouldOrderByRoleOrderAscending()
    {
        // Arrange
        GroupGovernorAggregate chiefFinancialOfficer = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000004",
            role: "Chief Financial Officer",
            roleOrder: 70);

        GroupGovernorAggregate trustee = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000002",
            role: "Trustee",
            roleOrder: 40);

        GroupGovernorAggregate governanceProfessional = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000005",
            role: "Governance Professional - Multi-Academy Trust (MAT)",
            roleOrder: 153);

        GroupGovernorAggregate chair = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000001",
            role: "Chair of Trustees",
            roleOrder: 30);

        GroupGovernorAggregate accountingOfficer = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000003",
            role: "Accounting Officer",
            roleOrder: 60);

        GroupGovernorAggregateToTrusteesMapper sut = new();

        // Act
        IReadOnlyCollection<Trustee> result = sut.Map(
        [
            chiefFinancialOfficer,
            trustee,
            governanceProfessional,
            chair,
            accountingOfficer
        ]);

        Trustee[] trustees = [.. result];

        // Assert
        Assert.Equal("1000001", trustees[0].Id.Value);
        Assert.Equal("1000002", trustees[1].Id.Value);
        Assert.Equal("1000003", trustees[2].Id.Value);
        Assert.Equal("1000004", trustees[3].Id.Value);
        Assert.Equal("1000005", trustees[4].Id.Value);
    }
}
