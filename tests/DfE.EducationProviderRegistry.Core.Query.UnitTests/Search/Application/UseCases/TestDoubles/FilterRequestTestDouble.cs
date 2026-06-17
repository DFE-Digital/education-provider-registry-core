using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal class FilterRequestTestDouble
{
    public static FilterRequest Fake()
    {
        Bogus.Faker faker = new();

        return new FilterRequest(
            filterName: faker.Name.JobType(),                               // Simulated filter name
            filterValues: [faker.Name.JobTitle(), faker.Name.JobTitle()]    // Simulated filter values
        );
    }
}
