using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchPipelineContextBuilder
{
    public static SearchPipelineContext BuildContext(
        ReadOnlyCollection<string>? ids,
        List<string>? facetNames)
    {
        SearchPipelineContext context = new();

        if (ids != null)
        {
            context.Set(ids);
        }

        if (facetNames != null)
        {
            context.Set(facetNames);
        }

        return context;
    }

    public static SearchPipelineContext BuildContext(
        List<(string FacetName, Task<IReadOnlyList<FacetResult>> Task)>? tasks)
    {
        SearchPipelineContext context = new();

        if (tasks != null)
        {
            context.Set(tasks);
        }

        return context;
    }

    public static SearchPipelineContext BuildContext(IReadOnlyList<Establishment>? establishments)
    {
        SearchPipelineContext context = new();

        if (establishments != null)
        {
            context.Set(establishments);
        }

        return context;
    }

    public static SearchPipelineContext BuildContext(ReadOnlyCollection<string>? ids)
    {
        SearchPipelineContext context = new();

        if (ids != null)
        {
            context.Set(ids);
        }

        return context;
    }

    public static SearchPipelineContext BuildContext(
        IReadOnlyList<Establishment>? establishments,
        Dictionary<string, int>? orderMap)
    {
        SearchPipelineContext context = new();

        if (establishments != null)
        {
            context.Set(establishments);
        }

        if (orderMap != null)
        {
            context.Set(orderMap);
        }

        return context;
    }
}
