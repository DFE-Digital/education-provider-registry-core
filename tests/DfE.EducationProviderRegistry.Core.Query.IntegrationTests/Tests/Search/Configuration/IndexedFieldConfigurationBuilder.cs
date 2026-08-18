using System.Reflection;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal sealed class IndexedFieldConfigurationBuilder
{
    private string _fieldChainingPredicate;
    private string? _name;
    private readonly List<(string behaviour, string behaviourChainingPredicate)> _behaviours;

    public IndexedFieldConfigurationBuilder()
    {
        _fieldChainingPredicate = "OR";
        _behaviours = [];
    }

    internal static IndexedFieldConfigurationBuilder Create() => new();
    internal IndexedFieldConfigurationBuilder WithChainingPredicate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Chaining predicate for field cannot be null or empty");
        }
        _fieldChainingPredicate = value;
        return this;
    }

    internal IndexedFieldConfigurationBuilder WithFieldName<TSearchedEntity>(string name) where TSearchedEntity : class
    {
        ValidateFieldExistsOn<TSearchedEntity>(name);
        _name = name;
        return this;
    }

    public IndexedFieldConfigurationBuilder WithExactMatchBehaviour(string behaviourChainingPredicate = "OR") => WithBehaviour("exact", behaviourChainingPredicate);
    public IndexedFieldConfigurationBuilder WithPartialMatchBehaviour(string behaviourChainingPredicate = "OR") => WithBehaviour("partial", behaviourChainingPredicate);
    public IndexedFieldConfigurationBuilder WithFuzzyMatchBehaviour(string behaviourChainingPredicate = "OR") => WithBehaviour("fuzzy", behaviourChainingPredicate);

    private IndexedFieldConfigurationBuilder WithBehaviour(string name, string behaviourChainingPredicate = "OR")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Behaviour name for field cannot be null or empty");
        }
        _behaviours.Add((name, behaviourChainingPredicate));
        return this;
    }

    public IndexedFieldConfiguration Build()
    {
        if (string.IsNullOrWhiteSpace(_name))
        {
            throw new ArgumentException("Field name cannot be null or empty when built");
        }

        return new IndexedFieldConfiguration()
        {
            FieldName = _name,
            ChainingPredicate = _fieldChainingPredicate,
            SearchBehaviours = MapBehavioursToConfiguration(_behaviours).ToArray()
        };
    }

    private static void ValidateFieldExistsOn<T>(string prop)
    {
        // Search uses reflection to construct Expressions, these expressions require the target field to exist on the reflected type
        Type targetType = typeof(T);
        PropertyInfo _ = targetType.GetProperty(prop) ?? throw new ArgumentException($"Property {prop} does not exist on {targetType.FullName}");
    }

    private static IEnumerable<SearchBehaviourConfiguration> MapBehavioursToConfiguration(IEnumerable<(string behaviour, string behaviourChainingPredicate)> behaviours)
    {
        return behaviours.Select((behaviour) =>
            new SearchBehaviourConfiguration()
            {
                Name = behaviour.behaviour,
                ChainingPredicate = behaviour.behaviourChainingPredicate
            });
    }
}
