namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.Compression;

public interface IFileCompressor
{
    byte[] CompressFile(params (string FileName, byte[] Content)[] files);
}
