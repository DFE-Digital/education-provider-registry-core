using System.IO.Compression;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure;

public sealed class FileCompressor : IFileCompressor
{
    public byte[] CompressFile(params (string FileName, byte[] Content)[] files)
    {
        using MemoryStream ms = new();
        using (ZipArchive zip = new(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach ((string FileName, byte[] Content) in files)
            {
                ArgumentNullException.ThrowIfNull(FileName);
                ArgumentNullException.ThrowIfNull(Content);

                ZipArchiveEntry entry = zip.CreateEntry(FileName);

                using Stream entryStream = entry.Open();
                entryStream.Write(Content, 0, Content.Length);
            }
        }

        return ms.ToArray();
    }
}
