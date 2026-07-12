using System.Runtime.CompilerServices;
using System.Text;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Formatters;

internal sealed class DefaultFilterExpressionFormatter : IFilterExpressionFormatter
{
    /// <summary>
    /// The string value used to separate placeholder tokens when generating
    /// composite filter expressions.
    /// </summary>
    private string ExpressionParamsSeparator { get; set; } = string.Empty;

    /// <summary>
    /// Sets the string used to separate placeholder tokens in the formatted
    /// filter expression.
    /// </summary>
    /// <param name="separator">
    /// The separator applied between placeholder tokens.
    /// </param>
    public void SetExpressionParamsSeparator(string separator)
    {
        ExpressionParamsSeparator = separator;
    }

    /// <summary>
    /// Creates a formatted filter expression by applying the supplied
    /// <paramref name="filterCriteria"/> to the provided <paramref name="expressionFormat"/>.
    /// The concrete syntax of the resulting expression depends on the template supplied.
    /// </summary>
    /// <param name="expressionFormat">
    /// A composite format string containing placeholder tokens (e.g., "{0}", "{1}").
    /// </param>
    /// <param name="filterCriteria">
    /// The values to be inserted into the formatted expression.
    /// </param>
    /// <returns>
    /// A formatted filter expression string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="expressionFormat"/> is null or whitespace.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when no filter criteria are provided.
    /// </exception>
    public string CreateFormattedExpression(string expressionFormat, params object[] filterCriteria)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(expressionFormat);

        if (filterCriteria.Length == 0)
        {
            throw new ArgumentException(
                "Filter argument cannot be null or empty", nameof(filterCriteria));
        }

        return FormattableStringFactory.Create(expressionFormat, filterCriteria).ToString();
    }

    /// <summary>
    /// Generates a placeholder string based on the number of filter criteria supplied.
    /// For example, with three criteria and a comma separator, the result would be:
    /// "{0},{1},{2}".
    /// </summary>
    /// <param name="filterCriteria">
    /// The collection used to determine the number of placeholder tokens.
    /// </param>
    /// <returns>
    /// A placeholder string such as "{0},{1},{2}".
    /// </returns>
    public string CreateFilterCriteriaPlaceholders(object[] filterCriteria)
    {
        StringBuilder filterCriteriaFormat = new();

        filterCriteriaFormat.AppendJoin(
            ExpressionParamsSeparator,
            Enumerable.Range(0, filterCriteria.Length)
                .Select(index => $"{{{index}}}"));

        return filterCriteriaFormat.ToString();
    }
}
