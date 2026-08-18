using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

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

    internal IndexedFieldConfigurationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    internal IndexedFieldConfigurationBuilder WithBehaviour(string name, string behaviourChainingPredicate = "OR")
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
