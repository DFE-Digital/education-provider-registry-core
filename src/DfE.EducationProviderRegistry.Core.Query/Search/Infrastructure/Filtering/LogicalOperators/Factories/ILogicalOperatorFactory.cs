namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;

public interface ILogicalOperatorFactory
{
    /// <summary>
    /// Allows creation of an <see cref="ILogicalOperator"/> instance based on the type name requested.
    /// </summary>
    /// <param name="logicalOperatorName">
    /// The name of the concrete implementation type <see cref="ILogicalOperator"/> requested.
    /// </param>
    /// <returns>
    /// The configured instance of the <see cref="ILogicalOperator"/> type.
    /// </returns>
    ILogicalOperator CreateLogicalOperator(string logicalOperatorName);
}
