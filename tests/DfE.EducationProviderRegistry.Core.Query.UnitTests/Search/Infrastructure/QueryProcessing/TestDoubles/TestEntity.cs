namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

public sealed class TestEntity
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public Address Address { get; set; } = new();
    public List<Site> Sites { get; set; } = [];
}

public sealed class Address
{
    public string Postcode { get; set; } = string.Empty;
}

public sealed class Site
{
    public string Code { get; set; } = string.Empty;
    public Location Location { get; set; } = new();
}

public sealed class Location
{
    public string Town { get; set; } = string.Empty;
}
