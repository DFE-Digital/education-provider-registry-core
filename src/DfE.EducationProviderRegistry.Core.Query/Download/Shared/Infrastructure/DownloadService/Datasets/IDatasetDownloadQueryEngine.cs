namespace DynamicCsvFileGeneration.Datasets;

public interface IDatasetDownloadQueryEngine<TInput> where TInput : class
{
    IQueryable<TProjection> BuildQuery<TProjection>(
        DatasetDownloadRequest request);
}