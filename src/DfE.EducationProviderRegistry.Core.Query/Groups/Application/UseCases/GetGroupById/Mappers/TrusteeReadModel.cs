using System;
using System.Collections.Generic;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

public sealed record TrusteeReadModel
{
    public required string Id { get; init; }
    public required string FullName { get; init; }
    public required DateTime StartDate { get; init; }
    public TrusteeTitleType? Title { get; init; }
}
