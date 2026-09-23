using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.Models;
using DfE.EducationProviderRegistry.Core.Query.Download.Datasets.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Application.UseCases.Response;

public sealed class DownloadDatasetsResponseTests
{
    [Fact]
    public void Constructor_WithDataset_SetsDownloadedDatasetProperty()
    {
        // arrange
        Dataset dataset =
            new(
                "file.csv",
                "text/csv",
                "csv",
                3,
                [1, 2, 3]
            );

        // act
        DownloadDatasetsResponse response = new(dataset);

        // assert
        Assert.Equal(dataset, response.DownloadedDataset);
    }

    [Fact]
    public void Constructor_WithNullDataset_SetsDownloadedDatasetToNull()
    {
        // arrange
        Dataset? dataset = null;

        // act
        DownloadDatasetsResponse response = new(dataset);

        // assert
        Assert.Null(response.DownloadedDataset);
    }

    [Fact]
    public void DownloadedDatasetProperty_IsReadOnly()
    {
        // arrange
        Dataset dataset =
            new(
                "file.csv",
                "text/csv",
                "csv",
                3,
                [1, 2, 3]
            );

        DownloadDatasetsResponse response = new(dataset);

        // act
        Dataset? value = response.DownloadedDataset;

        // assert
        Assert.Equal(dataset, value);
    }

    [Fact]
    public void Type_IsSealed()
    {
        // arrange
        Type type = typeof(DownloadDatasetsResponse);

        // act
        bool isSealed = type.IsSealed;

        // assert
        Assert.True(isSealed);
    }

    [Fact]
    public void DownloadedDatasetProperty_IsOfTypeDatasetNullable()
    {
        // arrange
        Type propertyType = typeof(DownloadDatasetsResponse)
            .GetProperty(nameof(DownloadDatasetsResponse.DownloadedDataset))!
            .PropertyType;

        // act
        bool isDataset = propertyType == typeof(Dataset);

        // assert
        Assert.True(isDataset);
    }
}
