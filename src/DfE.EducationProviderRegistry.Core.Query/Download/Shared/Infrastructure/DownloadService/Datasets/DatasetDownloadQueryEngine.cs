using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Datasets.Options;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;
using Microsoft.Extensions.Options;

namespace DynamicCsvFileGeneration.Datasets;

public sealed class DatasetDownloadQueryEngine<TInput>  :
    IDatasetDownloadQueryEngine<TInput> where TInput : class
{
    private readonly DownloadOptions _downloadOptions;
    private readonly ISpecificationConfigurationBuilder<TInput>
        _specificationConfigurationBuilder;
    private readonly IQueryEngine _queryEngine;

    public DatasetDownloadQueryEngine(
        IOptions<DownloadOptions> downloadOptions,
        ISpecificationConfigurationBuilder<TInput>
            specificationConfigurationBuilder,
        IQueryEngine queryEngine)
    {
        _downloadOptions = downloadOptions.Value;
        _specificationConfigurationBuilder =
            specificationConfigurationBuilder;
        _queryEngine = queryEngine;
    }

    public IQueryable<TProjection> BuildQuery<TProjection>(
        DatasetDownloadRequest request)
    {
        if (!_downloadOptions.DownloadProfiles.TryGetValue(
                request.ProfileKey,
                out DownloadProfileConfig? config))
        {
            throw new InvalidOperationException(
                $"Export profile '{request.ProfileKey}' was not found.");
        }

        ISpecification<TInput> specification =
            _specificationConfigurationBuilder
                .BuildFromConfig(config);

        Expression<Func<TInput, TProjection>>
            projection =
                ProjectionBuilder.BuildDynamicProjection<
                    TInput,
                    TProjection>(config.SelectedColumns);

        return _queryEngine
            .BuildQuery(specification, projection);
    }
}
