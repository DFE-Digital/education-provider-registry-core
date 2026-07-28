using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

/// <summary>
/// Writes string constant values into the SQL buffer during trigram‑based
/// expression translation, applying correct SQL escaping.
/// </summary>
public sealed class StringConstantTranslationStrategy : IConstantTranslationStrategy
{
    /// <summary>
    /// Determines whether the supplied value is a <see cref="string"/>.
    /// </summary>
    /// <param name="value">The constant value to evaluate.</param>
    /// <returns><c>true</c> if the value is a string; otherwise <c>false</c>.</returns>
    public bool CanHandle(object value) => value is string;

    /// <summary>
    /// Writes the string constant into the supplied <see cref="StringBuilder"/>,
    /// escaping embedded single quotes and surrounding the value with SQL
    /// string literal delimiters.
    /// </summary>
    /// <param name="value">The string constant to write.</param>
    /// <param name="sb">The SQL buffer being constructed.</param>
    public void Write(object value, StringBuilder sb)
    {
        ArgumentNullException.ThrowIfNull(sb);

        string? str = (string)value;
        sb.Append('\'').Append(str.Replace("'", "''")).Append('\'');
    }
}
