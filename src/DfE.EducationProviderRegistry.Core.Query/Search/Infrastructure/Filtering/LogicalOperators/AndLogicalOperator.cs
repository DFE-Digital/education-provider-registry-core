using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;

internal sealed class AndLogicalOperator : ILogicalOperator
{
    private const string LogicOperator = "AND";

    public string GetOperatorExpression() => LogicOperator.PadSides();
}
