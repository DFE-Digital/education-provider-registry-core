using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

public sealed class IndexedFieldConfigurationBuilder
{
    public const string AND_CHAINING_PREDICATE = "AND";
    public const string OR_CHAINING_PREDICATE = "OR";
    private string _fieldChainingPredicate;
    private string? _name;
    private readonly List<(string behaviour, string? behaviourChainingPredicate)> _behaviours;

    public IndexedFieldConfigurationBuilder()
    {
        _fieldChainingPredicate = OR_CHAINING_PREDICATE; // DEFAULT assume each field should chain
        _behaviours = [];
    }

    public static IndexedFieldConfigurationBuilder Create() => new();

    public IndexedFieldConfigurationBuilder WithFieldName(string name)
    {
        _name = name;
        return this;
    }

    public IndexedFieldConfigurationBuilder WithFieldDefaultBehaviourChainingAnd() => WithFieldChainingPredicate(AND_CHAINING_PREDICATE);
    public IndexedFieldConfigurationBuilder WithFieldDefaultBehaviourChainingOr() => WithFieldChainingPredicate(OR_CHAINING_PREDICATE);

    private IndexedFieldConfigurationBuilder WithFieldChainingPredicate(string value)
    {
        _fieldChainingPredicate = value;
        return this;
    }

    public IndexedFieldConfigurationBuilder AppendExactMatchBehaviour(string? behaviourChainingPredicate = null) => WithBehaviour("exact", behaviourChainingPredicate);
    public IndexedFieldConfigurationBuilder AppendContainsMatchBehaviour(string? behaviourChainingPredicate = null) => WithBehaviour("contains", behaviourChainingPredicate);
    public IndexedFieldConfigurationBuilder AppendFuzzyMatchBehaviour(string? behaviourChainingPredicate = null) => WithBehaviour("fuzzy", behaviourChainingPredicate);

    private IndexedFieldConfigurationBuilder WithBehaviour(string name, string? behaviourChainingPredicate = null)
    {
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
            DefaultBehaviourChainingPredicate = _fieldChainingPredicate,
            SearchBehaviours = MapBehavioursToConfiguration(_behaviours).ToArray()
        };
    }

    private static IEnumerable<SearchBehaviourConfiguration> MapBehavioursToConfiguration(IEnumerable<(string behaviour, string? behaviourChainingPredicate)> behaviours)
    {
        return behaviours.Select((behaviour) =>
            new SearchBehaviourConfiguration()
            {
                Name = behaviour.behaviour,
                ChainingPredicate = behaviour.behaviourChainingPredicate
            });
    }
}
