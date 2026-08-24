using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed record ResolvedPath(
    ParameterExpression RootParameter,
    Expression AccessExpression,
    bool IsCollection,
    ParameterExpression? CollectionElementParameter,
    string? CollectionNavigationName
);
