namespace DfE.EducationProviderRegistry.Core.Query.DownloadDatasets.Infrastructure;

public interface IFileCompressor
{
    byte[] CompressFile(params (string FileName, byte[] Content)[] files);
}
