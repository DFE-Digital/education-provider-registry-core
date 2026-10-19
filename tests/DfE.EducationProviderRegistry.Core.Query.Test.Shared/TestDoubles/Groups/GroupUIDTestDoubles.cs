using System.Security.Cryptography;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

public static class GroupUIDTestDoubles
{
    public static GroupUID Stub() => Create(1000);

    public static GroupUID Create()
    {
        return Create(
            RandomNumberGenerator.GetInt32(10_000));
    }

    public static GroupUID Create(int value)
    {
        return new GroupUID(value);
    }
}
