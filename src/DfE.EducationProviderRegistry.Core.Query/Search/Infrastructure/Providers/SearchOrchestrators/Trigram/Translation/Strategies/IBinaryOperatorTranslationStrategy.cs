using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

/// <summary>
/// Defines a strategy for writing a binary operator token into a SQL buffer
/// during trigram‑based expression translation.
/// </summary>
public interface IBinaryOperatorTranslationStrategy
{
    /// <summary>
    /// Writes the operator token into the supplied <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The SQL buffer being constructed.</param>
    void Write(StringBuilder sb);
}
