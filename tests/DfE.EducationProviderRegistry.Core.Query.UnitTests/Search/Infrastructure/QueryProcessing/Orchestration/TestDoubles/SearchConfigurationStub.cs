using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;

public static class SearchConfigurationStub
{
    public static SearchConfiguration CreateDefault()
    {
        return new SearchConfiguration
        {
            Keys = new List<SearchIndexKeyConfiguration>
            {
                new() {
                    SearchTermKey = "Name",
                    FieldChainingPredicate = "AND",
                    IndexedFields = new List<IndexedFieldConfiguration>
                    {
                        new() {
                            FieldName = "Name",
                            DefaultBehaviourChainingPredicate = "AND",
                            SearchBehaviours = new List<SearchBehaviourConfiguration>
                            {
                                new() {
                                    Name = "Equals",
                                    ChainingPredicate = "AND"
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    public static SearchConfiguration CreatePerson()
    {
        return new SearchConfiguration
        {
            Keys = new List<SearchIndexKeyConfiguration>
            {
                new() {
                    SearchTermKey = "Person",
                    FieldChainingPredicate = "AND",
                    IndexedFields = new List<IndexedFieldConfiguration>
                    {
                        new() {
                            FieldName = "Name",
                            DefaultBehaviourChainingPredicate = "AND",
                            SearchBehaviours = new List<SearchBehaviourConfiguration>
                            {
                                new() {
                                    Name = "Equals",
                                    ChainingPredicate = "AND"
                                }
                            }
                        },
                        new() {
                            FieldName = "Age",
                            DefaultBehaviourChainingPredicate = "AND",
                            SearchBehaviours = new List<SearchBehaviourConfiguration>
                            {
                                new() {
                                    Name = "Equals",
                                    ChainingPredicate = "AND"
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
