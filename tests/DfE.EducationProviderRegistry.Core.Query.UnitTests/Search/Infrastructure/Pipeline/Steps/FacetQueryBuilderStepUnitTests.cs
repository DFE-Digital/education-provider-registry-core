using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps;

public sealed class FacetQueryBuilderStepUnitTests
{
    private static SearchPipelineContext BuildContext(
        params (string FacetName, Task<IReadOnlyList<FacetResult>> Task)[] tasks)
    {
        SearchPipelineContext context = new();
        context.Set(new List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>(tasks));
        return context;
    }

    [Fact]
    public void Execute_Throws_WhenTaskNotCompleted()
    {
        // arrange
        Task<IReadOnlyList<FacetResult>> incompleteTask = new(() => []);

        SearchPipelineContext context =
            BuildContext(("phase", incompleteTask));

        FacetQueryBuilderStep step = new();

        // act // assert
        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(() =>
                step.Execute(context, CancellationToken.None));

        Assert.Contains("not completed", ex.Message);
    }

    [Fact]
    public void Execute_Throws_WhenTaskFaulted()
    {
        // arrange
        Task<IReadOnlyList<FacetResult>> faultedTask =
            Task.FromException<IReadOnlyList<FacetResult>>(new InvalidOperationException("boom"));

        SearchPipelineContext context =
            BuildContext(("phase", faultedTask));

        FacetQueryBuilderStep step = new();

        // act // assert
        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(() =>
                step.Execute(context, CancellationToken.None));

        Assert.Contains("failed", ex.Message);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Equal("boom", ex.InnerException!.Message);
    }

    [Fact]
    public void Execute_Throws_WhenFacetResultsAreNull()
    {
        // arrange
        Task<IReadOnlyList<FacetResult>> nullResultTask =
            Task.FromResult<IReadOnlyList<FacetResult>>(null!);

        SearchPipelineContext context =
            BuildContext(("phase", nullResultTask));

        FacetQueryBuilderStep step = new();

        // act // assert
        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(() =>
                step.Execute(context, CancellationToken.None));

        Assert.Contains("returned null results", ex.Message);
    }

    [Fact]
    public void Execute_Throws_WhenCancellationRequested()
    {
        // arrange
        IReadOnlyList<FacetResult> results = [new("Primary", 10)];

        Task<IReadOnlyList<FacetResult>> completedTask =
            Task.FromResult(results);

        SearchPipelineContext context =
            BuildContext(("phase", completedTask));

        FacetQueryBuilderStep step = new();
        using CancellationTokenSource cts = new();
        cts.Cancel();

        // act // assert
        Assert.Throws<OperationCanceledException>(() =>
            step.Execute(context, cts.Token));
    }

    [Fact]
    public void Execute_SetsFacets_WhenTasksAreValid()
    {
        // arrange
        IReadOnlyList<FacetResult> results = [new("Primary", 10)];

        Task<IReadOnlyList<FacetResult>> completedTask =
            Task.FromResult(results);

        SearchPipelineContext context =
            BuildContext(("phase", completedTask));

        FacetQueryBuilderStep step = new();

        // act
        step.Execute(context, CancellationToken.None);

        // assert
        List<SearchFacet> facets = context.Get<List<SearchFacet>>();

        Assert.Single(facets);
        Assert.Equal("phase", facets[0].Name);
        Assert.Single(facets[0].Results);
        Assert.Equal("Primary", facets[0].Results[0].Value);
        Assert.Equal(10, facets[0].Results[0].Count);
    }
}
