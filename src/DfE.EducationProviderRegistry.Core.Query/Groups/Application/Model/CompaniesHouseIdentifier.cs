using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record CompaniesHouseIdentifier
{
    public CompaniesHouseIdentifier(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value.Trim();
    }

    public string Value { get; }
}
