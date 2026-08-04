using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

/// <summary>
/// Writes constant values directly into the SQL buffer during trigram‑based
/// expression translation. This strategy handles all constant types.
/// </summary>
public sealed class ConstantTranslationStrategy : IConstantTranslationStrategy
{
    /// <summary>
    /// Indicates that this strategy can handle any constant value.
    /// </summary>
    /// <param name="value">The constant value to evaluate.</param>
    /// <returns>Always <c>true</c>.</returns>
    public bool CanHandle(object value) => true;

    /// <summary>
    /// Writes the constant value into the supplied <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="value">The constant value to write.</param>
    /// <param name="sb">The SQL buffer being constructed.</param>
    public void Write(object value, StringBuilder sb)
    {
        ArgumentNullException.ThrowIfNull(sb);
        sb.Append(value);
    }

}
