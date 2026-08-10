using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps;

public sealed class FacetQueryResolverStepUnitTests
{
    [Fact]
    public async Task HandleAsync_Throws_WhenFacetTasksMissing()
    {
        // arrange
        FacetQueryResolverStep step = new();

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(tasks: null);

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
    public async Task HandleAsync_Throws_WhenCancellationRequested()
    {
        // arrange
        IReadOnlyList<FacetResult> results = [new("1", "Primary", 10)];

        Task<IReadOnlyList<FacetResult>> completedTask =
            Task.FromResult(results);

        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
        [
            ("phase", completedTask)
        ];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(tasks);

        FacetQueryResolverStep step = new();

        using CancellationTokenSource cts = new();

        await cts.CancelAsync();

        // act // assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => step.HandleAsync(
                context,
                cts.Token).AsTask());
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenAnyTaskFaults()
    {
        // arrange
        Task<IReadOnlyList<FacetResult>> faultedTask =
            Task.FromException<IReadOnlyList<FacetResult>>(
                new InvalidOperationException("boom"));

        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
        [
            ("phase", faultedTask)
        ];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(tasks);

        FacetQueryResolverStep step = new();

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains("failed", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Equal("boom", ex.InnerException!.Message);
    }

    [Fact]
    public async Task HandleAsync_CompletesSuccessfully_WhenAllTasksComplete()
    {
        // arrange
        IReadOnlyList<FacetResult> results =
            [new FacetResult("1", "Primary", 10)];

        Task<IReadOnlyList<FacetResult>> completedTask =
            Task.FromResult(results);

        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
        [
            ("phase", completedTask),
            ("type", completedTask)
        ];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(tasks);

        FacetQueryResolverStep step = new();

        // act
        await step.HandleAsync(
            context,
            TestContext.Current.CancellationToken);

        // assert
        Assert.True(true);
    }
}
