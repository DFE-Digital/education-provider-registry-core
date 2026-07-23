using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

/// <summary>
/// Writes SQL <c>NULL</c> for constant values that are <c>null</c> during
/// trigram‑based expression translation.
/// </summary>
public sealed class NullConstantTranslationStrategy : IConstantTranslationStrategy
{
    /// <summary>
    /// Determines whether the supplied value is <c>null</c>.
    /// </summary>
    /// <param name="value">The constant value to evaluate.</param>
    /// <returns><c>true</c> if the value is <c>null</c>; otherwise <c>false</c>.</returns>
    public bool CanHandle(object value) => value is null;

    /// <summary>
    /// Writes the SQL <c>NULL</c> literal into the supplied <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="value">The constant value (ignored).</param>
    /// <param name="sb">The SQL buffer being constructed.</param>
    public void Write(object value, StringBuilder sb) => sb.Append("NULL");
}
