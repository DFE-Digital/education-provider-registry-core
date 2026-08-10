namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed record ParsedPropertyPath(
    bool IsCollection,
    string NavigationName,
    string RemainderPath
);

