using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;

public sealed record MemberReadModel
{
    public required string Identifier { get; init; }
    public required string FullName { get; init; }
    public required DateTime StartDate { get; init; }
}
