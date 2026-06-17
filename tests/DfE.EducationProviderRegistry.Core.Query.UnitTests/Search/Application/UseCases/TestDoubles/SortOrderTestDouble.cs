using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SortOrderTestDouble
{
    public static SortOrder Stub() =>
        new(
            sortField: "Forename",                          // Primary sort field
            sortDirection: "desc",                          // Sort direction
            validSortFields: ["Forename", "Surname", "DOB"] // Allowed fields for sorting
        );
}
