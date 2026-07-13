namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

/// <summary>
/// Generates an equality-based filter expression for a single value or multiple values.
/// </summary>
/// <remarks>
/// Produces SQL fragments of the form:
/// <list type="bullet">
/// <item><description><c>column = 'value'</c> for a single value</description></item>
/// <item><description><c>(column = 'a' OR column = 'b')</c> for multiple values</description></item>
/// </list>
/// Null, empty, or whitespace values are ignored.
/// Strings are safely quoted and escaped. Booleans are emitted as <c>TRUE</c>/<c>FALSE</c>.
/// </remarks>
internal sealed class SingleOrMultiValueEqualsExpression : ISearchFilterExpression
{
    /// <summary>
    /// Builds an equality filter expression for the provided filter request.
    /// </summary>
    /// <param name="searchFilterRequest">The filter request containing the column and values.</param>
    /// <returns>
    /// A SQL equality expression. Returns an empty string when all values are ignored.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="searchFilterRequest"/> is null.</exception>
    public string GetFilterExpression(SearchFilterRequest searchFilterRequest)
    {
        ArgumentNullException.ThrowIfNull(searchFilterRequest);

        List<string> expressions =
            BuildExpressions(
                column: searchFilterRequest.FilterKey,
                values: searchFilterRequest.FilterValues);

        if (expressions.Count == 0)
        {
            return string.Empty;
        }

        if (expressions.Count == 1)
        {
            return expressions[0];
        }

        return string.Concat(
            LogicalOperators.Open,
            string.Join(LogicalOperators.Or, expressions),
            LogicalOperators.Close
        );
    }

    /// <summary>
    /// Builds individual equality expressions for each valid value.
    /// </summary>
    /// <param name="column">The column name to compare against.</param>
    /// <param name="values">The raw filter values.</param>
    /// <returns>A list of formatted equality expressions.</returns>
    private static List<string> BuildExpressions(string column, object[] values)
    {
        List<string> expressions = new(values.Length);

        for (int i = 0; i < values.Length; i++)
        {
            object formatted = FormatValue(values[i]);

            if (ReferenceEquals(formatted, NoValue.Instance))
            {
                continue;
            }

            expressions.Add(
                string.Concat(column, LogicalOperators.Eq, formatted.ToString()));
        }

        return expressions;
    }

    /// <summary>
    /// Formats a value into its SQL literal representation.
    /// </summary>
    /// <param name="value">The raw value.</param>
    /// <returns>
    /// A formatted SQL literal, or <see cref="NoValue.Instance"/> when the value should be ignored.
    /// </returns>
    private static object FormatValue(object value)
    {
        return value switch
        {
            null => NoValue.Instance,

            string s when string.IsNullOrWhiteSpace(s)
                => NoValue.Instance,

            string s
                => LogicalOperators.Quote
                   + s.Replace(LogicalOperators.Quote, LogicalOperators.Quote + LogicalOperators.Quote)
                   + LogicalOperators.Quote,

            bool b
                => b ? LogicalOperators.TrueLiteral : LogicalOperators.FalseLiteral,

            _ => FormatNonString(value)
        };
    }

    /// <summary>
    /// Formats non-string values, ignoring null or empty representations.
    /// </summary>
    private static object FormatNonString(object value) =>
        (value is null || string.IsNullOrWhiteSpace(value.ToString())) ? NoValue.Instance : value;

    /// <summary>
    /// Represents a sentinel value indicating that a filter value should be ignored.
    /// </summary>
    private sealed class NoValue
    {
        public static readonly NoValue Instance = new();
        private NoValue() { }
    }

    /// <summary>
    /// Defines the SQL operators used when constructing equality expressions.
    /// </summary>
    private readonly struct LogicalOperators
    {
        public const string Or = " OR ";
        public const string Eq = " = ";
        public const string Open = "(";
        public const string Close = ")";
        public const string Quote = "'";
        public const string TrueLiteral = "TRUE";
        public const string FalseLiteral = "FALSE";
    }
}
