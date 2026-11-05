using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;

public interface IQueryEngine
{
    IQueryable<object> BuildQuery(string fileKey);
}
