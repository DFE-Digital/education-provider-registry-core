using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed class GroupDataToGroupMapper : IMapper<GroupData, Group>
{
    public Group Map(GroupData input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return null;
        //GroupIdentity identity = new(
        //            new GroupId(input.),
        //            new GroupUID(input.GroupRecord.GroupId)
        //        );

        //GroupExternalIdentifiers externalIds = new(
        //// map from identifiers collection
        //);

        //GroupComposition composition = new(
        //// academies, members, trustees
        //);

        //GroupCharacteristics characteristics = new(
        //    input.GroupRecord.Name,
        //    // address from contact etc
        //);

        //return new Group(identity, externalIds, composition, characteristics);

    }
}
