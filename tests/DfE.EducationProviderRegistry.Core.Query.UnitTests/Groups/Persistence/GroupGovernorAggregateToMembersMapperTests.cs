using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Persistence;

public sealed class GroupGovernorAggregateToMembersMapperTests
{
    [Fact]
    public void Map_GivenNullInput_ShouldThrowArgumentNullException()
    {
        // Arrange
        GroupGovernorAggregateToMembersMapper sut = new();

        Func<IReadOnlyCollection<Member>> act = () => sut.Map(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Map_GivenEmptyInput_ShouldReturnEmptyCollection()
    {
        // Arrange
        GroupGovernorAggregateToMembersMapper sut = new();

        IEnumerable<GroupGovernorAggregate> input = [];

        // Act
        IReadOnlyCollection<Member> result = sut.Map(input);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_GivenNonMemberGovernor_ShouldExcludeGovernor()
    {
        // Arrange
        GroupGovernorAggregate governor = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1234567",
            role: "Trustee");

        GroupGovernorAggregateToMembersMapper sut = new();

        // Act
        IReadOnlyCollection<Member> result = sut.Map([governor]);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Map_GivenMemberGovernor_ShouldMapAllProperties()
    {
        // Arrange
        GroupGovernorAggregate governor = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1234567",
            firstName: "David",
            lastName: "Evans",
            role: "Member",
            roleOrder: 50,
            title: "Mr",
            appointingBody: "Appointed by academy members",
            appointmentStartDate: new DateOnly(2016, 12, 5),
            appointmentEndDate: new DateOnly(2028, 8, 27));

        GroupGovernorAggregateToMembersMapper sut = new();

        // Act
        IReadOnlyCollection<Member> result = sut.Map([governor]);

        // Assert
        Member member = Assert.Single(result);

        Assert.Equal("1234567", member.Id.Value);
        Assert.Equal("David Evans", member.Name.Value);
        Assert.Equal("Mr", member.Title);
        Assert.Equal("Appointed by academy members", member.AppointedBy);
        Assert.Equal(new DateOnly(2016, 12, 5), member.StartDate);
        Assert.Equal(new DateOnly(2028, 8, 27), member.EndDate);
    }

    [Fact]
    public void Map_GivenMemberWithNoEndDate_ShouldMapNullEndDate()
    {
        // Arrange
        GroupGovernorAggregate governor = GroupGovernorAggregateTestDoubles.CreateWith(
            role: "Member",
            appointmentEndDate: null);

        GroupGovernorAggregateToMembersMapper sut = new();

        // Act
        IReadOnlyCollection<Member> result = sut.Map([governor]);

        // Assert
        Member member = Assert.Single(result);

        Assert.Null(member.EndDate);
    }

    [Fact]
    public void Map_GivenMultipleGovernorRoles_ShouldOnlyMapMembers()
    {
        // Arrange
        GroupGovernorAggregate member = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1000205",
            role: "Member");

        GroupGovernorAggregate trustee = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1548386",
            role: "Trustee");

        GroupGovernorAggregate accountingOfficer = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "1716699",
            role: "Accounting Officer");

        GroupGovernorAggregateToMembersMapper sut = new();

        // Act
        IReadOnlyCollection<Member> result = sut.Map(
            [trustee, member, accountingOfficer]);

        // Assert
        Member mappedMember = Assert.Single(result);

        Assert.Equal("1000205", mappedMember.Id.Value);
    }

    [Fact]
    public void Map_GivenMultipleMembers_ShouldOrderByRoleOrderAscending()
    {
        // Arrange
        GroupGovernorAggregate lowerPriority = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "2000001",
            role: "Member",
            roleOrder: 50);

        GroupGovernorAggregate higherPriority = GroupGovernorAggregateTestDoubles.CreateWith(
            governorId: "2000002",
            role: "Member",
            roleOrder: 10);

        GroupGovernorAggregateToMembersMapper sut = new();

        // Act
        IReadOnlyCollection<Member> result = sut.Map(
            [lowerPriority, higherPriority]);

        Member[] members = [.. result];

        // Assert
        Assert.Equal("2000002", members[0].Id.Value);
        Assert.Equal("2000001", members[1].Id.Value);
    }
}
