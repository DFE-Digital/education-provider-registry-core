using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

public sealed record FacetDefinition<TEntity>(
    Expression<Func<TEntity, object>> Selector,
    Expression<Func<TEntity, string>>? AdditionalValueSelector = null)
 where TEntity : class;
