using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Behaviours;

public interface ISearchBehaviour
{
    string Name { get; }

    Expression<Func<Establishment, bool>> Build(string propertyPath, string value);
}
