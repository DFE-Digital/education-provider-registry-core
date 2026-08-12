using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

/// <summary>
/// Defines a strategy for writing constant values into a SQL buffer during
/// trigram‑based expression translation.
/// </summary>
public interface IConstantTranslationStrategy
{
    /// <summary>
    /// Determines whether this strategy can handle the specified constant value.
    /// </summary>
    /// <param name="value">The constant value to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the strategy can handle the value; otherwise <c>false</c>.
    /// </returns>
    bool CanHandle(object value);

    /// <summary>
    /// Writes the constant value into the supplied <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="value">The constant value to write.</param>
    /// <param name="sb">The SQL buffer being constructed.</param>
    void Write(object value, StringBuilder sb);
}
