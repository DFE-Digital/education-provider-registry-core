using DfE.Core.Libraries.DesignPatterns.Specification;
using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.DownloadService;

public record FileDownloadRequest<TEntity, TProjection>(
    ISpecification<TEntity> specifications,
    Expression<Func<TEntity, TProjection>> projection,
    string OutputPath) where TEntity: class;
