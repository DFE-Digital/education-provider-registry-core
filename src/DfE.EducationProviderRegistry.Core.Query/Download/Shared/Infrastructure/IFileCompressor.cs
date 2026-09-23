namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure;

public interface IFileCompressor
{
    byte[] CompressFile(params (string FileName, byte[] Content)[] files);
}
