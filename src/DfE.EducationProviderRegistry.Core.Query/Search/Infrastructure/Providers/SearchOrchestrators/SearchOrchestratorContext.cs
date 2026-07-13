using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

public sealed record SearchOrchestratorContext
{
    /// <summary>
    /// The raw search term provided by the caller. This value is typically used as the
    /// primary input to trigram similarity search or other full‑text search mechanisms.
    /// </summary>
    /// <remarks>
    /// This property is required and must contain a non‑empty value. Validation is expected
    /// to occur at the MVC boundary before constructing the context.
    /// </remarks>
    public required string SearchTerm { get; init; }

    /// <summary>
    /// The database column to apply the search term against. When empty, the orchestrator
    /// may fall back to a provider‑specific default search column.
    /// </summary>
    /// <remarks>
    /// This value is optional and allows callers to override the default search column
    /// configured by the provider. It is typically used when multiple searchable fields
    /// exist within the same entity.
    /// </remarks>
    public string SearchColumn { get; init; } = string.Empty;

    /// <summary>
    /// The maximum number of records to return for the current search request.
    /// </summary>
    /// <remarks>
    /// This value is used to generate the SQL <c>LIMIT</c> clause or equivalent paging
    /// mechanism in the underlying search provider.
    /// </remarks>
    public int PageSize { get; init; }

    /// <summary>
    /// The number of records to skip before returning results. This value is used to
    /// generate the SQL <c>OFFSET</c> clause or equivalent paging mechanism.
    /// </summary>
    /// <remarks>
    /// Typically calculated as <c>(PageNumber - 1) * PageSize</c> by the MVC layer.
    /// </remarks>
    public int Offset { get; init; }

    /// <summary>
    /// The collection of filter requests to apply to the search operation. Each filter
    /// request defines a filter key and associated values that are resolved into a concrete
    /// <see cref="ISearchFilterExpression"/> instance by the configured filter expression
    /// factory.
    /// </summary>
    /// <remarks>
    /// When empty, no additional filtering is applied beyond the primary search term.
    /// </remarks>
    public IReadOnlyList<SearchFilterRequest> Filters { get; init; } = [];
}
