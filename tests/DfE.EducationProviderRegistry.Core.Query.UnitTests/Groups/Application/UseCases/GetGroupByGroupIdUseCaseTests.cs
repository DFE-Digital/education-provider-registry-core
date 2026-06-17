using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using Microsoft.Extensions.Logging;
using Microsoft.Testing.Platform.Requests;
using Moq;
using Tests.Shared;
using Tests.Shared.Logger;
using Tests.Shared.Mapper;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class GetGroupByGroupIdUseCaseTests
{
    private readonly CancellationToken _ctx;
    public GetGroupByGroupIdUseCaseTests()
    {
        _ctx = TestContext.Current.CancellationToken;
    }

    [Fact]
    public void Constructor_Should_Throw_When_Logger_Is_Null()
    {
        // Arrange
        Func<GetGroupByGroupIdUseCase> construct =
            () => new(
                    null!,
                    MockTestDouble.Default<IGroupsRepository>().Object,
                    IMapperTestDouble.Default<Group, GroupDto>());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Repository_Is_Null()
    {
        // Arrange
        Func<GetGroupByGroupIdUseCase> construct =
            () => new(
                    ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
                    null!,
                    IMapperTestDouble.Default<Group, GroupDto>());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Mapper_Is_Null()
    {
        // Arrange
        Func<GetGroupByGroupIdUseCase> construct =
            () => new(
                    ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
                    MockTestDouble.Default<IGroupsRepository>().Object,
                    null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_When_Request_Is_Null()
    {
        // Arrange
        GetGroupByGroupIdUseCase sut = CreateSut(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            MockTestDouble.Default<IGroupsRepository>().Object,
            IMapperTestDouble.Default<Group, GroupDto>());

        // Act
        UseCaseResponse<GroupDto> result =
            await sut.HandleRequestAsync(null!, _ctx);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal("The request cannot be null.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_When_Group_Not_Found()
    {
        // Arrange
        Mock<IGroupsRepository> repository = MockTestDouble.For<IGroupsRepository, Group?>(
            (repo) => repo.GetGroupByGroupIdAsync(It.IsAny<GroupId>(), It.IsAny<CancellationToken>()),
            null!);

        GetGroupByGroupIdUseCase sut = CreateSut(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            IMapperTestDouble.Default<Group, GroupDto>());

        // Act
        GetGroupByGroupIdRequest request = StubRequest();

        UseCaseResponse<GroupDto> result =
            await sut.HandleRequestAsync(request, _ctx);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal($"Group with GroupId {request.GroupId} not found.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Success_When_Group_Found()
    {
        // Arrange
        Group stubGroup = new GroupBuilder().Build();

        Mock<IGroupsRepository> repository = MockTestDouble.For<IGroupsRepository, Group?>(
            (repo) => repo.GetGroupByGroupIdAsync(It.IsAny<GroupId>(), It.IsAny<CancellationToken>()),
            stubGroup);

        GroupDto dto = StubGroupDto();

        Mock<IMapper<Group, GroupDto>> mapper = IMapperTestDouble.Map<Group, GroupDto>(dto);

        GetGroupByGroupIdUseCase sut = CreateSut(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            mapper.Object);

        GetGroupByGroupIdRequest request = StubRequest();

        // Act
        UseCaseResponse<GroupDto> result =
            await sut.HandleRequestAsync(request, _ctx);

        // Assert
        Assert.True(result.SuccessfulRequest);
        Assert.Null(result.ErrorMessage);
        Assert.Same(dto, result.Model);

        // Assert - control flow
        repository.Verify(
            (repo) => repo.GetGroupByGroupIdAsync(
                It.Is<GroupId>((identifier) => identifier.Value == request.GroupId),
                _ctx));

        mapper.Verify(m => m.Map(stubGroup), Times.Once);
    }

    //[Fact]
    //public async Task HandleRequestAsync_Should_Return_Failure_When_Operation_Cancelled()
    //{
    //    // Arrange
    //    Mock<IGroupsRepository> repository = CreateRepository();

    //    repository
    //        .Setup(r => r.GetGroupByGroupIdAsync(It.IsAny<GroupIdentifier>(), It.IsAny<CancellationToken>()))
    //        .ThrowsAsync(new OperationCanceledException());

    //    GetGroupByGroupIdUseCase sut = new(
    //        CreateLogger().Object,
    //        repository.Object,
    //        CreateMapper().Object);

    //    GetGroupByGroupIdRequest request = new("test-id");

    //    // Act
    //    UseCaseResponse<GroupDto> result =
    //        await sut.HandleRequestAsync(request);

    //    // Assert
    //    Assert.False(result.IsSuccess);
    //}

    //[Fact]
    //public async Task HandleRequestAsync_Should_Return_Failure_When_Invalid_GroupIdentifier()
    //{
    //    // Arrange
    //    GetGroupByGroupIdUseCase sut = new(
    //        CreateLogger().Object,
    //        CreateRepository().Object,
    //        CreateMapper().Object);

    //    GetGroupByGroupIdRequest request = new("invalid!!!");

    //    // Act
    //    UseCaseResponse<GroupDto> result =
    //        await sut.HandleRequestAsync(request);

    //    // Assert
    //    Assert.False(result.IsSuccess);
    //}

    //[Fact]
    //public async Task HandleRequestAsync_Should_Return_Failure_On_Unexpected_Exception()
    //{
    //    // Arrange
    //    Mock<IGroupsRepository> repository = CreateRepository();

    //    repository
    //        .Setup(r => r.GetGroupByGroupIdAsync(It.IsAny<GroupIdentifier>(), It.IsAny<CancellationToken>()))
    //        .ThrowsAsync(new Exception("boom"));

    //    GetGroupByGroupIdUseCase sut = new(
    //        CreateLogger().Object,
    //        repository.Object,
    //        CreateMapper().Object);

    //    GetGroupByGroupIdRequest request = new("test-id");

    //    // Act
    //    UseCaseResponse<GroupDto> result =
    //        await sut.HandleRequestAsync(request);

    //    // Assert
    //    Assert.False(result.IsSuccess);
    //}

    private static GetGroupByGroupIdRequest StubRequest() => new("Any group id");

    private static GetGroupByGroupIdUseCase CreateSut(ILogger<GetGroupByGroupIdUseCase> logger, IGroupsRepository repository, IMapper<Group, GroupDto> mapper)
        => new(logger, repository, mapper);

    private static GroupDto StubGroupDto()
    {
        return new()
        {
            GroupId = "Id",
            GroupUID = 123,
            CompaniesHouseId = "A123",
            Academies = [],
            Members = [],
            Trustees = []
        };
    }
}
