namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;

public interface IQueryEngine
{
    IQueryable<object> BuildQuery(string fileKey);
}
