using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

/// <summary>
/// Writes a binary operator token (e.g., AND, OR, =, <>) into a SQL fragment
/// during trigram‑based search translation.
/// </summary>
public sealed class BinaryOperatorTranslationStrategy : IBinaryOperatorTranslationStrategy
{
    private readonly string _token;

    /// <summary>
    /// Creates a new operator translation strategy using the specified token.
    /// </summary>
    /// <param name="token">The operator token to write (e.g., "AND", "=").</param>
    public BinaryOperatorTranslationStrategy(string token)
    {
        _token = token;
    }

    /// <summary>
    /// Writes the operator token into the supplied <see cref="StringBuilder"/>,
    /// padded with surrounding spaces for correct SQL formatting.
    /// </summary>
    /// <param name="sb">The SQL buffer being constructed.</param>
    public void Write(StringBuilder sb)
    {
        ArgumentNullException.ThrowIfNull(sb);
        sb.Append(' ').Append(_token).Append(' ');
    }
}
