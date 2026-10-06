using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal sealed class SearchReadModelBuilder
{
    private string _id = "123456";
    private string _name = "Test Establishment";
    private string _typeName = "Test Type";
    private int _typeId = 123;
    private string? _address = "Test Address Line, Test Address Line 2, Test City, Test County, AA1 1AA";
    private string _localAuthorityName = "Test Local Authority";
    private string _groupCode = "GROUP1";
    private string _groupName = "Test Group";
    private string _providerCategory = "Establishment";
    private int _academyCount = 2;

    public static SearchReadModelBuilder Create() => new();

    public SearchReadModelBuilder WithId(string value)
    {
        _id = value;
        return this;
    }

    public SearchReadModelBuilder WithName(string value)
    {
        _name = value;
        return this;
    }

    public SearchReadModelBuilder WithTypeName(string value)
    {
        _typeName = value;
        return this;
    }

    public SearchReadModelBuilder WithTypeId(int value)
    {
        _typeId = value;
        return this;
    }

    public SearchReadModelBuilder WithAddress(string? value)
    {
        _address = value;
        return this;
    }

    public SearchReadModelBuilder WithGroup(string code, string name)
    {
        _groupCode = code;
        _groupName = name;
        return this;
    }

    public SearchReadModelBuilder WithProviderCategory(string category)
    {
        _providerCategory = category;
        return this;
    }

    public SearchReadModelBuilder WithLocalAuthorityName(string laName)
    {
        _localAuthorityName = laName;
        return this;
    }

    public SearchReadModelBuilder WithAcademyCount(int value)
    {
        _academyCount = value;
        return this;
    }

    public SearchReadModel Build()
    {
        return new(
            Id: _id,
            Name: _name,
            TypeName: _typeName,
            TypeId: _typeId,
            Address: _address!,
            LocalAuthorityName: _localAuthorityName,
            GroupCode: _groupCode,
            GroupName: _groupName,
            ProviderCategory: _providerCategory,
            AcademyCount: _academyCount);
    }
}
