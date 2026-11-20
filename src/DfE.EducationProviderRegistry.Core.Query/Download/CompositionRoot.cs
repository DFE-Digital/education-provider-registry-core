using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService.Exporters.Csv;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.Download;

/// <summary>
/// Registers application-level and infrastructure-level dependencies
/// required for download-focused behaviour.
/// </summary>
public static class CompositionRoot
{
    public static IServiceCollection AddDownloadDatasetDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<
            IUseCase<
                DownloadDatasetsRequest,
                UseCaseResponse<DownloadDatasetsResponse>>,
            DownloadDatasetsUseCase>();

        services.TryAddScoped<
            IDatasetDownloadRepository,
            DatasetDownloadRepository>();

        services.TryAddScoped<
            IQueryEngine,
            StubQueryEngine>();

        services.TryAddTransient<
            IDownloadExporter,
            PostgresCsvDownloadExporter>();

        return services;
    }
}
