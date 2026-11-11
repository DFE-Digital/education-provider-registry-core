using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;

public sealed class StubQueryEngine : IQueryEngine
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public StubQueryEngine(EducationProviderRegistryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<object> BuildQuery(string fileKey)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(fileKey);

        return fileKey switch
        {
            "open-provider" =>
                _dbContext.EstablishmentAggregate
                    .AsNoTracking()
                    .Where(sa => sa.StatusCode == 1)    // open.
                    .Select(sa => new EstablishmentDownloadDto
                    {
                        EstablishmentUniqueReferenceNumberUrn = sa.Urn,
                        EstablishmentNumber = sa.EstablishmentNumber,
                        EstablishmentName = sa.Name,
                        EstablishmentTypeName = sa.EstablishmentTypeName,
                        EducationPhaseName = sa.EducationPhaseName
                        //ProviderName
                        //LocalAuthorityID
                        //LocalAuthority
                        //PreviousLocalAuthorityID
                        //PreviousLocalAuthority
                        //EducationPhaseID
                        //EducationPhase
                        //ProviderTypeID
                        //ProviderType
                        //FurtherEducationType
                        //ApprovedIndependentSENDSchoolSection41
                        //ProviderStatusID
                        //ProviderStatus
                        //OpenDate
                        //ReasonProviderOpenedID
                        //ReasonProviderOpened
                        //ClosedDate
                        //ReasonProviderClosedID
                        //ReasonProviderClosed
                        //UniqueReferenceNumber
                        //UKProviderReferenceNumber
                        //ProviderNumber
                        //PreviousProviderNumber
                        //NumberOfPupilsStudents
                        //NumberOfMales
                        //NumberOfFemales
                        //ProviderCapacity
                        //PupilsEligibleForFreeSchoolMeals
                        //PercentagePupilsEligibleForFreeSchoolMeals
                        //SpecialPupilsOnStatementOrEhcPlan
                        //SpecialPupilsNotOnStatementOrEhcPlan
                        //VisualImpairmentSEN1
                        //HearingImpairmentSEN2
                        //SpeechLanguageCommunicationSEN3
                        //AutisticSpectrumDisorderSEN4
                        //SocialEmotionalMentalHealthSEN5
                        //MultiSensoryImpairmentSEN6
                        //PhysicalDisabilitySEN7
                        //ModerateLearningDifficultySEN8
                        //SevereLearningDifficultySEN9
                        //ProfoundMultipleLearningDifficultySEN10
                        //SpecificLearningDifficultySEN11
                        //OtherDifficultyDisabilitySEN12
                        //TypeOfResourcedProvision
                        //ResourcedProvisionPupilCount
                        //ResourcedProvisionCapacity
                        //SENUnitPupilCount
                        //SenUnitCapacity
                        //HeadTitle
                        //HeadFirstName
                        //HeadLastName
                        //JobTitle
                        //UniquePropertyReferenceNumber
                        //Proprietor
                        //Site
                        //AddressLine1
                        //AddressLine2
                        //AddressLine3
                        //TownOrCity
                        //County
                        //Postcode
                        //Country
                        //Website
                        //PhoneNumber
                        //InspectorateName
                        //BritishSchoolOverseasInspectorateName
                        //InspectorateReport
                        //DateOfLastInspectionVisit
                        //DateOfNextInspectionVisit
                        //SixthFormProvisionID
                        //SixthFormProvision
                        //NurseryProvision
                        //StatutoryLowAge
                        //StatutoryHighAge
                        //SpecialClassProvisionID
                        //SpecialClassProvision
                        //AdmissionsPolicyID
                        //AdmissionsPolicy
                        //GenderOfEntryID
                        //GenderOfEntry
                        //BoardingEstablishment
                        //BoardingProvisionID
                        //BoardingProvision
                        //CensusDate
                        //DateLastChangedOrConfirmed
                        //DistrictAdministrativeID
                        //DistrictAdministrative
                        //AdministrativeWardID
                        //AdministrativeWard
                        //ParliamentaryConstituencyID
                        //ParliamentaryConstituency
                        //UrbanRuralClassificationID
                        //UrbanRuralClassification
                        //GovernmentStatisticalServiceLocalAuthorityCode
                        //Easting
                        //Northing
                        //MiddleSuperOutputAreaID
                        //MiddleSuperOutputArea
                        //LowerSuperOutputAreaID
                        //LowerSuperOutputArea
                        //GovernmentOfficeRegionID
                        //GovernmentOfficeRegion
                        //ReligiousCharacterID
                        //ReligiousCharacter
                        //ReligiousEthos
                        //DioceseID
                        //Diocese
                        //ProviderTypeGroupID
                        //ProviderTypeGroup
                        //TrustSchoolFlagID
                        //TrustSchoolFlag
                        //TrustsID
                        //Trusts
                        //FederationFlag
                        //FederationID
                        //Federation
                        //CompaniesHouseNumber
                        //QualificationsAwardingBodyID
                        //QualificationsAwardingBody
                        //QualificationsAwardingBodyReport
                        //ProviderAccreditedID
                        //ProviderAccredited
                        //AccreditationExpiryDate
                        //SchoolSponsorFlag
                        //SchoolSponsor
                        //FurtherEducationHigherEducationIdentifier
                        //TeenMothersProvision
                        //TeenMothersCapacity
                        //ChildcareFacilitiesProvision
                        //SENPupilReferralUnit
                        //EmotionalBehaviouralDifficultiesProvision
                        //PupilReferralUnitCapacity
                        //FullTimeProvision
                        //PupilsEducatedByOtherProviders
                    })
                    .Cast<object>(),

            "closed-provider" =>
                _dbContext.EstablishmentAggregate
                    .AsNoTracking()
                    .Where(sa => sa.StatusCode == 2)    // closed.
                    .Select(sa => new EstablishmentDownloadDto
                    {
                        EstablishmentUniqueReferenceNumberUrn = sa.Urn,
                        EstablishmentNumber = sa.EstablishmentNumber,
                        EstablishmentName = sa.Name,
                        EstablishmentTypeName = sa.EstablishmentTypeName,
                        EducationPhaseName = sa.EducationPhaseName
                    })
                    .Cast<object>(),

            "open-provider-group" =>
                _dbContext.GroupAggregate
                    .AsNoTracking()
                    .Select(ga => new GroupDownloadDto
                    {
                        GroupUniqueIdentifierUID = ga.GroupUid.ToString(),
                        GroupId = ga.GroupId ?? string.Empty,
                        GroupName = ga.Name ?? string.Empty,
                        GroupType = ga.GroupTypeName ?? string.Empty,
                        //GroupTypeID = string.Empty, // Map from aggregate if available
                        GroupStatusID = ga.GroupStatusLabel == "Closed" ? "CLOSED" : "OPEN",
                        GroupStatus = ga.GroupStatusLabel ?? string.Empty,
                        OpenDate =
                            (
                                ga.GroupTypeName != "Single-academy trust" &&
                                ga.GroupTypeName != "Multi-academy trust"
                            ) &&
                            ga.GroupStatusEffectiveDate.HasValue
                                ? ga.GroupStatusEffectiveDate.Value.ToString("yyyy-MM-dd")
                                : string.Empty,
                        ClosedDate = string.Empty,
                        IncorporatedOn =
                            (
                                ga.GroupTypeName == "Single-academy trust" ||
                                ga.GroupTypeName == "Multi-academy trust"
                            ) &&
                            ga.GroupStatusEffectiveDate.HasValue
                                ? ga.GroupStatusEffectiveDate.Value.ToString("yyyy-MM-dd")
                                : string.Empty,
                        CompaniesHouseNumber = ga.CompaniesHouseNumber ?? string.Empty,
                        UKProviderReferenceNumber = ga.Ukprn ?? string.Empty,
                        //HeadOfGroupTitle = string.Empty,
                        //HeadOfGroupFirstName = string.Empty,
                        //HeadOfGroupLastName = ga.HeadteacherName ?? string.Empty,
                        GroupAddressLine1 = ga.SiteName ?? string.Empty,
                        GroupAddressLine2 = ga.AddressLine1 ?? string.Empty,
                        GroupAddressLine3 = ga.AddressLine2 ?? string.Empty,
                        GroupTownOrCity = ga.Town ?? string.Empty,
                        GroupCounty = ga.County ?? string.Empty,
                        GroupPostcode = ga.Postcode ?? string.Empty
                    })
                    .Cast<object>(),
                        _ => throw new ArgumentException($"Unknown file key: {fileKey}", nameof(fileKey))
                    };
    }
}
