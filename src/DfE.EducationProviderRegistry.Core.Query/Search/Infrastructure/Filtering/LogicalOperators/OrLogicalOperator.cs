using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;

internal sealed class OrLogicalOperator : ILogicalOperator
{
    private const string LogicOperator = "OR";

    public string GetOperatorExpression() => LogicOperator.PadSides();
}
