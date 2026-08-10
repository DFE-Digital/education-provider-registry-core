using System.Collections.ObjectModel;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps;

public sealed class FacetQueryDispatchStepUnitTests
{
    [Fact]
    public async Task HandleAsync_Throws_WhenIdsMissing()
    {
        // arrange
        Mock<IFacetProvider> providerMock =
            FacetProviderTestDouble.Mock();

        FacetQueryDispatchStep step = new(providerMock.Object);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                null,
                ["phase"]);

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
    public async Task HandleAsync_Throws_WhenFacetNamesMissing()
    {
        // arrange
        Mock<IFacetProvider> providerMock =
            FacetProviderTestDouble.Mock();

        FacetQueryDispatchStep step = new(providerMock.Object);

        ReadOnlyCollection<string> ids = new(["10001"]);

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                ids,
                null);

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
    public async Task HandleAsync_Throws_WhenFacetNameIsEmpty()
    {
        // arrange
        Mock<IFacetProvider> providerMock =
            FacetProviderTestDouble.Mock();

        FacetQueryDispatchStep step = new(providerMock.Object);

        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = [""];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                ids,
                facetNames);

        // act // assert
        InvalidOperationException ex =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => step.HandleAsync(
                    context,
                    CancellationToken.None).AsTask());

        Assert.Contains(
            "Facet name cannot be null or empty",
            ex.Message);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCancellationRequested()
    {
        // arrange
        Mock<IFacetProvider> providerMock =
            FacetProviderTestDouble.Mock();

        FacetQueryDispatchStep step = new(providerMock.Object);

        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = ["phase"];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                ids,
                facetNames);

        using CancellationTokenSource cts = new();

        await cts.CancelAsync();

        // act // assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => step.HandleAsync(
                context,
                cts.Token).AsTask());
    }

    [Fact]
    public async Task HandleAsync_SetsEmptyTaskList_WhenFacetNamesEmpty()
    {
        // arrange
        Mock<IFacetProvider> providerMock =
            FacetProviderTestDouble.Mock();

        FacetQueryDispatchStep step = new(providerMock.Object);

        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = [];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                ids,
                facetNames);

        // act
        await step.HandleAsync(
            context,
            TestContext.Current.CancellationToken);

        // assert
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        Assert.Empty(tasks);
    }

    [Fact]
    public async Task HandleAsync_DispatchesFacetTasksCorrectly()
    {
        // arrange
        Mock<IFacetProvider> providerMock =
            FacetProviderTestDouble.MockFor([new FacetResult("1", "Primary", 10)]);
        FacetQueryDispatchStep step = new(providerMock.Object);

        ReadOnlyCollection<string> ids = new(["10001"]);
        List<string> facetNames = ["phase", "type"];

        SearchPipelineContext context =
            SearchPipelineContextBuilder.BuildContext(
                ids,
                facetNames);

        // act
        await step.HandleAsync(
            context,
            TestContext.Current.CancellationToken);

        // assert
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)> tasks =
            context.Get<List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>>();

        Assert.Equal(2, tasks.Count);
        Assert.Equal("phase", tasks[0].FacetName);
        Assert.Equal("type", tasks[1].FacetName);

        IReadOnlyList<FacetResult> results0 =
            await tasks[0].Task;

        IReadOnlyList<FacetResult> results1 =
            await tasks[1].Task;

        Assert.Single(results0);
        Assert.Single(results1);
        Assert.Equal("Primary", results0[0].Value);
        Assert.Equal(10, results0[0].Count);
    }
}
