using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Insert;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal sealed class SearchEstablishmentFactory : ISearchEstablishmentFactory
{
    private readonly IInsertEstablishmentHandler _insertEstablishmentHandler;

    public SearchEstablishmentFactory(IInsertEstablishmentHandler insertEstablishmentHandler)
    {
        ArgumentNullException.ThrowIfNull(insertEstablishmentHandler);
        _insertEstablishmentHandler = insertEstablishmentHandler;
    }

    public async Task<IReadOnlyCollection<Establishment>> CreateManyAsync(int totalToCreate, int matchingSearchTermCount, string searchTerm, CancellationToken ct = default)
    {
        List<Establishment> establishments = [];

        for (int count = 0; count < totalToCreate; count++)
        {
            EstablishmentBuilder builder = new();
            builder.WithName(
                (count < matchingSearchTermCount) ?
                    CreateMatchingName(searchTerm, count) :
                      CreateNonMatchingName(count));

            establishments.Add(builder.Build());
        }

        await _insertEstablishmentHandler.InsertAsync(establishments, ct);
        return establishments;
    }


    private static string CreateMatchingName(
        string searchTerm,
        int index)
    {
        return $"{searchTerm}-match-{index}";
    }

    private static string CreateNonMatchingName(
        int index)
    {
        return $"ZZZ-{index}";
    }

}
