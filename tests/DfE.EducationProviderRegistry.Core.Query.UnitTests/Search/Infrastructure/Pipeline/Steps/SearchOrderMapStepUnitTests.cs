using System.Collections.ObjectModel;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps;

public sealed class SearchOrderMapStepUnitTests
{
    [Fact]
    public async Task HandleAsync_Throws_WhenUrnListMissing()
    {
        // arrange
        SearchOrderMapStep step = new();

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids: null);

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
    public async Task HandleAsync_Throws_WhenUrnIsNullOrEmpty()
    {
        // arrange
        ReadOnlyCollection<string> ids =
            new(["10001", "", "10003"]);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids);

        SearchOrderMapStep step = new();

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains(
            "null or empty URN",
            ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCancellationRequested()
    {
        // arrange
        ReadOnlyCollection<string> ids =
            new(["10001", "10002", "10003"]);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids);

        SearchOrderMapStep step = new();

        using CancellationTokenSource cts = new();

        await cts.CancelAsync();

        // act // assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => step.HandleAsync(
                context,
                cts.Token).AsTask());
    }

    [Fact]
    public async Task HandleAsync_CreatesCorrectOrderMap()
    {
        // arrange
        ReadOnlyCollection<string> ids =
            new(["10001", "10002", "10003"]);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(ids);

        SearchOrderMapStep step = new();

        // act
        await step.HandleAsync(
            context,
            TestContext.Current.CancellationToken);

        // assert
        Dictionary<string, int> orderMap =
            context.Get<Dictionary<string, int>>();

        Assert.Equal(3, orderMap.Count);
        Assert.Equal(0, orderMap["10001"]);
        Assert.Equal(1, orderMap["10002"]);
        Assert.Equal(2, orderMap["10003"]);
    }
}
