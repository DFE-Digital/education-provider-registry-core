using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download;

public sealed class DownloadCompositionRootTests
{
    [Fact]
    public void AddDownloadDatasetDependencies_NullServices_ThrowsArgumentNullException()
    {
        // arrange
        IServiceCollection? services = null;

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                CompositionRoot.AddDownloadDatasetDependencies(services!));

        // assert
        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddDownloadDatasetDependencies_RegistersDownloadDatasetsUseCase()
    {
        // arrange
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();

        // act
        CompositionRoot.AddDownloadDatasetDependencies(services);

        // assert
        ServiceProvider provider = services.BuildServiceProvider();

        IUseCase<
            DownloadDatasetsRequest,
            UseCaseResponse<DownloadDatasetsResponse>> resolved =
                provider.GetRequiredService<IUseCase<
                    DownloadDatasetsRequest,
                    UseCaseResponse<DownloadDatasetsResponse>>>();

        Assert.IsType<DownloadDatasetsUseCase>(resolved);
    }

    [Fact]
    public void AddDownloadDatasetDependencies_RegistersDatasetDownloadRepository()
    {
        // arrange
        IServiceCollection services = new ServiceCollection();

        // act
        CompositionRoot.AddDownloadDatasetDependencies(services);

        // assert
        ServiceProvider provider = services.BuildServiceProvider();

        IDatasetDownloadRepository resolved =
            provider.GetRequiredService<IDatasetDownloadRepository>();

        Assert.IsType<FakeDatasetDownloadRepository>(resolved);
    }

    [Fact]
    public void AddDownloadDatasetDependencies_RegistersFileCompressor()
    {
        // arrange
        IServiceCollection services = new ServiceCollection();

        // act
        CompositionRoot.AddDownloadDatasetDependencies(services);

        // assert
        ServiceProvider provider = services.BuildServiceProvider();

        IFileCompressor resolved =
            provider.GetRequiredService<IFileCompressor>();

        Assert.IsType<FileCompressor>(resolved);
    }

    [Fact]
    public void AddDownloadDatasetDependencies_RegistersDependenciesWithScopedLifetime()
    {
        // arrange
        IServiceCollection services = new ServiceCollection();

        // act
        CompositionRoot.AddDownloadDatasetDependencies(services);

        // assert
        ServiceDescriptor useCaseDescriptor =
            Assert.Single(services, serviceDescriptor =>
                serviceDescriptor.ServiceType ==
                typeof(IUseCase<
                    DownloadDatasetsRequest,
                    UseCaseResponse<DownloadDatasetsResponse>>));

        Assert.Equal(ServiceLifetime.Scoped, useCaseDescriptor.Lifetime);

        ServiceDescriptor repositoryDescriptor =
            Assert.Single(services, serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IDatasetDownloadRepository));

        Assert.Equal(ServiceLifetime.Scoped, repositoryDescriptor.Lifetime);

        ServiceDescriptor compressorDescriptor =
            Assert.Single(services, serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(IFileCompressor));

        Assert.Equal(ServiceLifetime.Scoped, compressorDescriptor.Lifetime);
    }

    [Fact]
    public void AddDownloadDatasetDependencies_DoesNotRegisterDuplicates()
    {
        // arrange
        IServiceCollection services = new ServiceCollection();

        // act
        CompositionRoot.AddDownloadDatasetDependencies(services);
        CompositionRoot.AddDownloadDatasetDependencies(services); // second call should not duplicate

        // assert
        int useCaseCount = 0;
        int repositoryCount = 0;
        int compressorCount = 0;

        foreach (ServiceDescriptor descriptor in services)
        {
            if (descriptor.ServiceType ==
                typeof(IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>))
            {
                useCaseCount++;
            }

            if (descriptor.ServiceType == typeof(IDatasetDownloadRepository))
            {
                repositoryCount++;
            }

            if (descriptor.ServiceType == typeof(IFileCompressor))
            {
                compressorCount++;
            }
        }

        Assert.Equal(1, useCaseCount);
        Assert.Equal(1, repositoryCount);
        Assert.Equal(1, compressorCount);
    }
}
