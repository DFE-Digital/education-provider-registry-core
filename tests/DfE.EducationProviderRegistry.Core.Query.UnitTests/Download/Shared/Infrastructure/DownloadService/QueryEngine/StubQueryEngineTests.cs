using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.QueryEngine.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.QueryEngine;

public sealed class StubQueryEngineTests : IDisposable
{
    private readonly StubDbContext _database;
    private readonly StubQueryEngine _sut;

    public StubQueryEngineTests()
    {
        _database = new StubDbContext();
        _sut = new StubQueryEngine(_database.DbContext);
    }

    public void Dispose()
    {
        _database.Dispose();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void BuildQuery_NullOrWhitespaceFileKey_ThrowsArgumentException(
        string? invalidFileKey)
    {
        Assert.Throws<ArgumentException>(
            () => _sut.BuildQuery(invalidFileKey!));
    }

    [Theory]
    [InlineData(null)]
    public void BuildQuery_NullFileKey_ThrowsArgumentNullException(
        string? invalidFileKey)
    {
        Assert.Throws<ArgumentNullException>(
            () => _sut.BuildQuery(invalidFileKey!));
    }

    [Fact]
    public void BuildQuery_UnknownFileKey_ThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => _sut.BuildQuery("unknown-key"));

        Assert.Contains("Unknown file key", exception.Message);
    }

    [Fact]
    public void BuildQuery_OpenProviderFileKey_ReturnsFilteredOpenEstablishments()
    {
        IQueryable<object> query =
            _sut.BuildQuery("open-provider");

        List<EstablishmentDownloadDto> results =
            [.. query.Cast<EstablishmentDownloadDto>()];

        Assert.Single(results);
        Assert.Equal("100001", results[0].EstablishmentUniqueReferenceNumberUrn);
        Assert.Equal("Open School", results[0].EstablishmentName);
    }

    [Fact]
    public void BuildQuery_ClosedProviderFileKey_ReturnsFilteredClosedEstablishments()
    {
        IQueryable<object> query =
            _sut.BuildQuery("closed-provider");

        List<EstablishmentDownloadDto> results =
            [.. query.Cast<EstablishmentDownloadDto>()];

        Assert.Single(results);
        Assert.Equal("100002", results[0].EstablishmentUniqueReferenceNumberUrn);
        Assert.Equal("Closed School", results[0].EstablishmentName);
    }

    [Fact]
    public void BuildQuery_OpenProviderGroupFileKey_ReturnsGroups()
    {
        IQueryable<object> query =
            _sut.BuildQuery("open-provider-group");

        List<GroupDownloadDto> results =
            [.. query.Cast<GroupDownloadDto>()];

        Assert.Single(results);
        Assert.Equal("500001", results[0].GroupUniqueIdentifierUID);
        Assert.Equal("Trust Alpha", results[0].GroupName);
    }
}
