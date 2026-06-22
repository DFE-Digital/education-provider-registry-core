using System;
using System.Collections.Generic;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

internal static class GroupCompositionTestDoubles
{
    internal static GroupComposition Create(
        IEnumerable<Academy>? academies = null,
        IEnumerable<Member>? members = null,
        IEnumerable<Trustee>? trustees = null)
    {
        return new GroupComposition(
            academies ?? [],
            members ?? [],
            trustees ?? []);
    }
}
