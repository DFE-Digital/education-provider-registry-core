using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps;

public sealed class SearchOrderingStepUnitTests
{
    [Fact]
    public async Task HandleAsync_Throws_WhenEstablishmentsMissing()
    {
        // arrange
        Dictionary<string, int> orderMap =
            new()
            {
                { "10001", 0 }
            };

        SearchPipelineContext context =
            SearchPipelineContextBuilder
                .BuildContext(null, orderMap);

        SearchOrderingStep step = new();

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains(
            "PipelineContext does not contain a value of type",
            ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenOrderMapMissing()
    {
        // arrange
        List<Establishment> establishments =
        [
            new Establishment { Urn = "10001" }
        ];

        SearchPipelineContext context =
            SearchPipelineContextBuilder
                .BuildContext(establishments, null);

        SearchOrderingStep step = new();

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains(
            "PipelineContext does not contain a value of type",
            ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenEstablishmentUrnIsNull()
    {
        // arrange
        List<Establishment> establishments =
        [
            new Establishment { Urn = null }
        ];

        Dictionary<string, int> orderMap = [];

        SearchPipelineContext context =
            SearchPipelineContextBuilder
                .BuildContext(establishments, orderMap);

        SearchOrderingStep step = new();

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains("null URN", ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenUrnNotInOrderMap()
    {
        // arrange
        List<Establishment> establishments =
        [
            new Establishment { Urn = "99999" }
        ];

        Dictionary<string, int> orderMap =
            new()
            {
                { "10001", 0 }
            };

        SearchPipelineContext context =
            SearchPipelineContextBuilder
                .BuildContext(establishments, orderMap);

        SearchOrderingStep step = new();

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains("99999", ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCancellationRequested()
    {
        // arrange
        List<Establishment> establishments =
        [
            new Establishment { Urn = "10001" }
        ];

        Dictionary<string, int> orderMap =
            new()
            {
                { "10001", 0 }
            };

        SearchPipelineContext context =
            SearchPipelineContextBuilder
                .BuildContext(establishments, orderMap);

        SearchOrderingStep step = new();

        using CancellationTokenSource cts = new();

        await cts.CancelAsync();

        // act // assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => step.HandleAsync(
                context,
                cts.Token).AsTask());
    }

    [Fact]
    public async Task HandleAsync_OrdersEstablishmentsCorrectly()
    {
        // arrange
        List<Establishment> establishments =
        [
            new Establishment { Urn = "B" },
            new Establishment { Urn = "A" },
            new Establishment { Urn = "C" }
        ];

        Dictionary<string, int> orderMap =
            new()
            {
                { "A", 0 },
                { "B", 1 },
                { "C", 2 }
            };

        SearchPipelineContext context =
            SearchPipelineContextBuilder
                .BuildContext(establishments, orderMap);

        SearchOrderingStep step = new();

        // act
        await step.HandleAsync(
            context,
            TestContext.Current.CancellationToken);

        // assert
        List<Establishment> ordered =
            context.Get<List<Establishment>>();

        Assert.Equal("A", ordered[0].Urn);
        Assert.Equal("B", ordered[1].Urn);
        Assert.Equal("C", ordered[2].Urn);
    }
}
