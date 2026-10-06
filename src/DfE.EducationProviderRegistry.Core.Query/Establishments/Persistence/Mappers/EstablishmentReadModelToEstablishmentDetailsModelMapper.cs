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

        EstablishmentLifecycleEventModel? openedEvent =
            establishment.OpenedDate is null
                ? null
                : new EstablishmentLifecycleEventModel(
                    EstablishmentLifecycleEventType.Opened,
                    establishment.OpenedDate.Value,
                    new EstablishmentLifeCycleReason(establishment.OpenedReason));

        EstablishmentLifecycleEventModel? closedEvent =
            establishment.ClosedDate is null
                ? null
                : new EstablishmentLifecycleEventModel(
                    EstablishmentLifecycleEventType.Closed,
                    establishment.ClosedDate.Value,
                    new EstablishmentLifeCycleReason(establishment.ClosedReason));

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
            establishment.LocalAuthorityName is null
                ? null
                : new LocalAuthority(
                    establishment.LocalAuthorityName,
                    establishment.LocalAuthorityCode ?? string.Empty);

        string? ageRange =
            establishment.StatutoryLowAge is not null &&
            establishment.StatutoryHighAge is not null
                ? $"{establishment.StatutoryLowAge} to {establishment.StatutoryHighAge}"
                : null;

        List<GovernorModel> governors =
        [
            .. input.Governors.Select(g => new GovernorModel(
            Identifier: new GovernanceIdentifier(g.GovernorId),
            Name: new Name(g.GovernorName)))
        ];

        EstablishmentContactDetails? contactDetails =
            establishment.Website is not null ||
            establishment.TelephoneNumber is not null
                ? new EstablishmentContactDetails(
                    establishment.Website ?? string.Empty,
                    establishment.TelephoneNumber ?? string.Empty)
                : null;

        EstablishmentInspection? ofsted =
            establishment.OfstedInspectionDate is null
            && establishment.OfstedReportUrl is null
                ? null
                : new EstablishmentInspection(establishment.OfstedInspectionDate, establishment.OfstedReportUrl);

        return new EstablishmentDetailsModel
        {
            Urn = EstablishmentUrnModel.Create(establishment.Urn),

            Name = new EstablishmentNameModel(establishment.Name),

            Number = establishment.EstablishmentNumber is null
                ? null
                : new EstablishmentNumberModel(establishment.EstablishmentNumber),

            Status = establishment.StatusName is null
                ? null
                : new EstablishmentStatusModel(establishment.StatusName),

            Type = establishment.EstablishmentTypeName is null
                ? null
                : new EstablishmentTypeModel(establishment.EstablishmentTypeName),

            Phase = establishment.EducationPhaseName is null
                ? null
                : new PhaseOfEducationModel(establishment.EducationPhaseName),

            LifecycleEventOpened = openedEvent,
            LifecycleEventClosed = closedEvent,

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
