using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.Download;

/// <summary>
/// Registers all application‑level and infrastructure‑level dependencies
/// required for all download focussed behaviour within the service.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers application‑layer and infrastructure-layer dataset download
    /// dependencies, including the <see cref="DownloadDatasetsUseCase"/>.
    /// </summary>
    public static IServiceCollection AddDownloadDatasetDependencies(
       this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .TryAddScoped<IUseCase<
                DownloadDatasetsRequest,
                UseCaseResponse<DownloadDatasetsResponse>>,
                DownloadDatasetsUseCase>();

        services
            .TryAddScoped<
                IDatasetDownloadRepository,
                FakeDatasetDownloadRepository>();

        services
            .TryAddScoped<
                IFileCompressor,
                FileCompressor>();

        return services;
    }
}
