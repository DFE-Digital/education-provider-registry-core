using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Download;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Infrastructure;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        ServiceCollection services = new();

        // 1. Register required configuration for PostgresCsvExporter
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["eprweb_eprdat_dotnet_db_connection"] = "Host=localhost;Database=test;"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        // 2. Register the missing DbContext required by StubQueryEngine or EF components
        // If you use EF Core, you can register a dummy options or a mock/in-memory context:
        services.AddDbContext<EducationProviderRegistryDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
        // Alternatively, if EducationProviderRegistryDbContext can be mocked:
        // services.AddTransient(_ => Mock.Of<EducationProviderRegistryDbContext>());

        // 3. Call your composition root extension method
        services.AddDownloadDatasetDependencies();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // act
        IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>? useCase =
            serviceProvider.GetService<IUseCase<DownloadDatasetsRequest, UseCaseResponse<DownloadDatasetsResponse>>>();

        // assert
        Assert.NotNull(useCase);
    }

    [Fact]
    public void AddDownloadDatasetDependencies_RegistersDatasetDownloadRepository()
    {
        // arrange
        // arrange
        ServiceCollection services = new();

        // 1. Register required configuration for PostgresCsvExporter
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["eprweb_eprdat_dotnet_db_connection"] = "Host=localhost;Database=test;"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        // 2. Register the missing DbContext required by StubQueryEngine or EF components
        // If you use EF Core, you can register a dummy options or a mock/in-memory context:
        services.AddDbContext<EducationProviderRegistryDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
        // Alternatively, if EducationProviderRegistryDbContext can be mocked:
        // services.AddTransient(_ => Mock.Of<EducationProviderRegistryDbContext>());


        // act
        CompositionRoot.AddDownloadDatasetDependencies(services);

        // assert
        ServiceProvider provider = services.BuildServiceProvider();

        IDatasetDownloadRepository resolved =
            provider.GetRequiredService<IDatasetDownloadRepository>();

        Assert.IsType<DatasetDownloadRepository>(resolved);
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
        }

        Assert.Equal(1, useCaseCount);
        Assert.Equal(1, repositoryCount);
    }
}
