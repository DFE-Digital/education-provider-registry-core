using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.UseCases;

public sealed class GetGroupByGroupIdUseCaseTests
{
    private readonly CancellationToken _ct;
    public GetGroupByGroupIdUseCaseTests()
    {
        _ct = TestContext.Current.CancellationToken;
    }

    [Fact]
    public void Constructor_Should_Throw_When_Logger_Is_Null()
    {
        // Arrange
        Func<GetGroupByGroupIdUseCase> construct =
            () => new(
                    null!,
                    MockTestDouble.Default<IGroupsRepository>().Object,
                    IMapperTestDouble.Default<Group, GroupReadModel>());

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
                    IMapperTestDouble.Default<Group, GroupReadModel>());

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
            IMapperTestDouble.Default<Group, GroupReadModel>());

        // Act
        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(null!, _ct);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal("The request cannot be null.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_When_RequestUid_Cannot_Be_Created()
    {
        // Arrange
        GetGroupByGroupIdUseCase sut = CreateSut(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            MockTestDouble.Default<IGroupsRepository>().Object,
            IMapperTestDouble.Default<Group, GroupReadModel>());

        // Act
        GetGroupByGroupUniqueIdentifierRequest request = new("INVALID_IDENTIFIER_THAT_FAILS_TRYCREATE");

        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(request, _ct);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal($"Could not parse the GroupUniqueIdentifier {request.GroupUid}", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_When_Group_Not_Found()
    {
        // Arrange
        Mock<IGroupsRepository> repository =
            MockTestDouble.For<
                IGroupsRepository, Group?>(
                    (repo) => repo.GetGroupByGroupUidAsync(
                        It.IsAny<GroupUID>(),
                        It.IsAny<CancellationToken>()),
                    null!);

        GetGroupByGroupIdUseCase sut = CreateSut(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            IMapperTestDouble.Default<Group, GroupReadModel>());

        // Act
        GetGroupByGroupUniqueIdentifierRequest request = StubRequest();

        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(request, _ct);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal($"Group with GroupId {request.GroupUid} not found.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Success_When_Group_Found()
    {
        // Arrange
        Group stubGroup = new GroupBuilder().Build();

        Mock<IGroupsRepository> repository =
            MockTestDouble.For<
                IGroupsRepository, Group?>(
                    (repo) => repo.GetGroupByGroupUidAsync(
                        It.IsAny<GroupUID>(),
                        It.IsAny<CancellationToken>()),
                    stubGroup);

        GroupReadModel dto = GroupReadModelTestDoubles.Stub();

        Mock<IMapper<Group, GroupReadModel>> mapper = IMapperTestDouble.Map<Group, GroupReadModel>(dto);

        GetGroupByGroupIdUseCase sut = CreateSut(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            mapper.Object);

        GetGroupByGroupUniqueIdentifierRequest request = StubRequest();

        // Act
        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(request, _ct);

        // Assert
        Assert.True(result.SuccessfulRequest);
        Assert.Null(result.ErrorMessage);
        Assert.Same(dto, result.Model);

        // Assert - control flow
        repository.Verify(
            (repo) => repo.GetGroupByGroupUidAsync(
                It.Is<GroupUID>((identifier) => identifier.Value.ToString() == request.GroupUid),
                _ct));

        mapper.Verify(m => m.Map(stubGroup), Times.Once);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_When_Operation_Cancelled()
    {
        // Arrange
        Mock<IGroupsRepository> repository =
            MockTestDouble.ThrowsExceptionFor<
                IGroupsRepository, Group?, OperationCanceledException>(
                    (repo) => repo.GetGroupByGroupUidAsync(
                        It.IsAny<GroupUID>(),
                        It.IsAny<CancellationToken>()));

        GetGroupByGroupIdUseCase sut = new(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            IMapperTestDouble.Default<Group, GroupReadModel>());

        GetGroupByGroupUniqueIdentifierRequest request = StubRequest();

        // Act
        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(request, _ct);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal("The operation was cancelled.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_When_Invalid_GroupIdentifier()
    {
        // Arrange
        InvalidGroupIdentifierException ex = new("Test");

        Mock<IGroupsRepository> repository =
            MockTestDouble.ThrowsExceptionFor<
                IGroupsRepository, Group?, InvalidGroupIdentifierException>(
                    (repo) => repo.GetGroupByGroupUidAsync(
                        It.IsAny<GroupUID>(),
                        It.IsAny<CancellationToken>()),
                    ex);

        GetGroupByGroupIdUseCase sut = new(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            IMapperTestDouble.Default<Group, GroupReadModel>());

        GetGroupByGroupUniqueIdentifierRequest request = StubRequest();

        // Act
        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(request, _ct);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal("Invalid group identifier.", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequestAsync_Should_Return_Failure_On_Unexpected_Exception()
    {
        // Arrange
        Mock<IGroupsRepository> repository =
            MockTestDouble.ThrowsExceptionFor<
                IGroupsRepository, Group?, Exception>(
                    (repo) => repo.GetGroupByGroupUidAsync(
                        It.IsAny<GroupUID>(),
                        It.IsAny<CancellationToken>()));

        GetGroupByGroupIdUseCase sut = new(
            ILoggerTestDouble.Default<GetGroupByGroupIdUseCase>(),
            repository.Object,
            IMapperTestDouble.Default<Group, GroupReadModel>());

        GetGroupByGroupUniqueIdentifierRequest request = StubRequest();

        // Act
        UseCaseResponse<GroupReadModel> result =
            await sut.HandleRequestAsync(request, _ct);

        // Assert
        Assert.False(result.SuccessfulRequest);
        Assert.Null(result.Model);
        Assert.Equal("An unexpected error occurred.", result.ErrorMessage);
    }

    private static GetGroupByGroupUniqueIdentifierRequest StubRequest() => new("1234");

    private static GetGroupByGroupIdUseCase CreateSut(
        ILogger<GetGroupByGroupIdUseCase> logger,
        IGroupsRepository repository,
        IMapper<Group, GroupReadModel> mapper) => new(logger, repository, mapper);
}
