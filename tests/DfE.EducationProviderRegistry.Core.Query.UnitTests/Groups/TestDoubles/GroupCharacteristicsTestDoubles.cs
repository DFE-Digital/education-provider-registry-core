using System;
using System.Collections.Generic;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupCharacteristicsTestDoubles
{
    internal static GroupCharacteristics Create()
    {
        return new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Stub(),
            GroupTypeTestDoubles.Create(),
            GroupStatusTestDoubles.Create());
    }
}
