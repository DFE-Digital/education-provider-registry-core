using System.IO.Compression;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Datasets.Infrastructure;

public sealed class FileCompressorTests
{
    private readonly FileCompressor _sut = new();

    [Fact]
    public void CompressFile_SingleFile_CompressesCorrectly()
    {
        // arrange
        string fileName = "test.txt";
        byte[] content = [1, 2, 3, 4];

        // act
        byte[] zipBytes = _sut.CompressFile((fileName, content));

        // assert
        using MemoryStream ms = new(zipBytes);
        using ZipArchive zip = new(ms, ZipArchiveMode.Read);

        Assert.Single(zip.Entries);

        ZipArchiveEntry entry = zip.Entries.First();
        Assert.Equal(fileName, entry.Name);

        using Stream entryStream = entry.Open();
        using BinaryReader reader = new(entryStream);
        byte[] extracted = reader.ReadBytes(content.Length);

        Assert.Equal(content, extracted);
    }

    [Fact]
    public void CompressFile_MultipleFiles_CompressesAllCorrectly()
    {
        // arrange
        (string, byte[])[] files =
        [
            ("a.txt", [10, 20]),
            ("b.txt", [30, 40, 50]),
            ("c.txt", [60])
        ];

        // act
        byte[] zipBytes = _sut.CompressFile(files);

        // assert
        using MemoryStream ms = new(zipBytes);
        using ZipArchive zip = new(ms, ZipArchiveMode.Read);

        Assert.Equal(3, zip.Entries.Count);

        foreach ((string? fileName, byte[]? content) in files)
        {
            ZipArchiveEntry entry =
                zip.Entries.Single(zipEntryArchive =>
                    zipEntryArchive.Name == fileName);

            using Stream entryStream = entry.Open();
            using BinaryReader reader = new(entryStream);
            byte[] extracted = reader.ReadBytes(content.Length);

            Assert.Equal(content, extracted);
        }
    }

    [Fact]
    public void CompressFile_NullFileName_ThrowsArgumentNullException()
    {
        // arrange
        (string, byte[])[] files =
        [
            ((string)null!, [1])
        ];

        // assert
        Assert.Throws<ArgumentNullException>(() => _sut.CompressFile(files));
    }

    [Fact]
    public void CompressFile_NullContent_ThrowsArgumentNullException()
    {
        // arrange
        (string, byte[])[] files =
        [
            ("test.txt", (byte[])null!)
        ];

        // assert
        Assert.Throws<ArgumentNullException>(() => _sut.CompressFile(files));
    }

    [Fact]
    public void CompressFile_NoFiles_ReturnsValidEmptyZip()
    {
        // act
        byte[] zipBytes = _sut.CompressFile();

        // assert
        using MemoryStream ms = new(zipBytes);
        using ZipArchive zip = new(ms, ZipArchiveMode.Read);

        Assert.Empty(zip.Entries);
        Assert.True(zipBytes.Length > 0);
    }
}

