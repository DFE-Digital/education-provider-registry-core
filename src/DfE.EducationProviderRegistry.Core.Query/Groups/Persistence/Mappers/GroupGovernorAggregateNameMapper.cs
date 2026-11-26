using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Persistence.Mappers;

internal static class GroupGovernorAggregateNameMapper
{
    internal static Name CreateName(GroupGovernorAggregate governor)
    {
        return new Name(
            string.Join(' ',
            new[] { governor.FirstName, governor.LastName }.Where((name) => !string.IsNullOrWhiteSpace(name))));
    }
}
