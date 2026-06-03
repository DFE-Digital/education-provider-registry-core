namespace IntegrationTests.Database.Server.Postgres.Container;

public sealed class PostgresContainerOptions
{
#nullable disable
    public string ImageName { get; set; }
#nullable enable
    public string? ImageTag { get; set; } = "latest";
    public string? Digest { get; set; }
    public string Image => string.IsNullOrEmpty(Digest) ? $"{ImageName}:{ImageTag}" : $"{ImageName}:{ImageTag}@{Digest}";
    public int? PublicPort { get; set; } = null;
    public IEnumerable<ServerArg> ServerArgs { get; set; } = [];
    public IEnumerable<ContainerResourceMapping>? CopyBeforeContainerInit { get; set; } = [];
}

public sealed record ServerArg(string Key, string[] Value);

public sealed record ContainerResourceMapping(
    string Source,
    string Destination,
    bool ReadOnly = true,
    bool Executable = false);
