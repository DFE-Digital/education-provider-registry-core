using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

internal sealed class EstablishmentReadModelToEstablishmentDetailsModelMapper : IMapper<EstablishmentReadModel, EstablishmentDetailsModel>
{
    public EstablishmentDetailsModel Map(EstablishmentReadModel input)
    {
        ArgumentNullException.ThrowIfNull(input);

        EstablishmentAggregate establishment = input.Establishment;

        EstablishmentGroupModel? group =
            establishment.GroupName is null
                ? null
                : new EstablishmentGroupModel(
                    establishment.GroupName,
                    establishment.GroupCode);

        SiteAddressModel? address =
            establishment.SiteName is null &&
            establishment.AddressLine1 is null &&
            establishment.Town is null &&
            establishment.Postcode is null
                ? null
                : new SiteAddressModel(
                    Name: establishment.SiteName ?? string.Empty,
                    AddressLine1: establishment.AddressLine1 ?? string.Empty,
                    AddressLine2: establishment.AddressLine2 ?? string.Empty,
                    Town: establishment.Town ?? string.Empty,
                    County: establishment.County ?? string.Empty,
                    Postcode: establishment.Postcode ?? string.Empty);

        LocalAuthority? localAuthority =
            establishment.LocalAuthorityCode is null || establishment.LocalAuthorityName is null
                ? null
                : new LocalAuthority(
                    establishment.LocalAuthorityName,
                    new LocalAuthorityCode(establishment.LocalAuthorityCode.Value));

        string? ageRange =
            establishment.StatutoryLowAge is not null &&
            establishment.StatutoryHighAge is not null
                ? $"{establishment.StatutoryLowAge} to {establishment.StatutoryHighAge}"
                : null;

        List<GovernorModel> governors =
        [
            .. input.Governors.Select((g) =>
                new GovernorModel(
                    Identifier: new GovernanceIdentifier(g.GovernorId),
                    Name: new Name(g.GovernorName)))
        ];

        EstablishmentContactDetails? contactDetails =
            establishment.Website is not null ||
            establishment.TelephoneNumber is not null
                ? new EstablishmentContactDetails(
                    Website: establishment.Website ?? string.Empty,
                    TelephoneNumber: establishment.TelephoneNumber ?? string.Empty)
                : null;

        EstablishmentInspection? ofsted =
            establishment.OfstedInspectionDate is null
            && establishment.OfstedReportUrl is null
                ? null
                : new EstablishmentInspection(
                    InspectionDate: establishment.OfstedInspectionDate,
                    ReportUrl: establishment.OfstedReportUrl);

        return new EstablishmentDetailsModel
        {
            Name = new EstablishmentNameModel(establishment.Name),

            Identifiers =
                new EstablishmentIdentifiersModel(
                    urn: new EstablishmentUrnModel(
                        new UniqueReferenceNumber(establishment.Urn)),
                    ukprn: establishment.Ukprn is null
                        ? null
                            : new Ukprn(establishment.Ukprn),
                    dfeNumber:
                        establishment.EstablishmentNumber is null || establishment.LocalAuthorityCode is null
                            ? null
                                : DfeNumber.Create(
                                    laCode: new LocalAuthorityCode(establishment.LocalAuthorityCode.Value),
                                    estabNumber: EstablishmentNumber.Parse(establishment.EstablishmentNumber))),

            Status = establishment.StatusCode is null
                ? null
                : new EstablishmentStatusModel(
                    status: (EstablishmentStatus)establishment.StatusCode,
                    effectiveDate: establishment.StatusDate),

            Type = establishment.EstablishmentTypeName is null
                ? null
                : new EstablishmentTypeModel(establishment.EstablishmentTypeName),

            Phase = establishment.EducationPhaseName is null
                ? null
                : new PhaseOfEducationModel(establishment.EducationPhaseName),

            Group = group,
            GroupType = establishment.GroupTypeName,
            GroupOpenDate = establishment.GroupOpenDate,

            Address = address,
            LocalAuthority = localAuthority,

            AgeRange = ageRange,
            Gender = establishment.Gender,

            ReligiousCharacter = establishment.ReligiousCharacter,

            Ofsted = ofsted,

            Governors = governors,

            Headteacher = establishment.HeadteacherName,

            SenProvision = establishment.SenProvision,

            ContactDetails = contactDetails
        };
    }
}
