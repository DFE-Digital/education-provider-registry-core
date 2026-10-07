using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentDetailsModel
{
    public required EstablishmentNameModel Name { get; init; }
    public required EstablishmentIdentifiersModel Identifiers { get; init; }

    public EstablishmentStatusModel? Status { get; init; }
    public EstablishmentTypeModel? Type { get; init; }
    public PhaseOfEducationModel? Phase { get; init; }

    public EstablishmentGroupModel? Group { get; init; }
    public string? GroupType { get; init; }
    public DateOnly? GroupOpenDate { get; init; }

    public IEnumerable<GovernorModel>? Governors { get; init; }

    public SiteAddressModel? Address { get; init; }

    public LocalAuthority? LocalAuthority { get; init; }

    public string? AgeRange { get; init; }

    public string? Gender { get; init; }

    public string? ReligiousCharacter { get; init; }

    public EstablishmentInspection? Ofsted { get; init; }

    public string? Headteacher { get; set; }

    public string? SenProvision { get; set; }

    public EstablishmentContactDetails? ContactDetails { get; set; }
    public EstablishmentPupils? Pupils { get; set; }
}
