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
                    ChainingPredicate = "AND",
                    IndexedFields = new List<IndexedFieldConfiguration>
                    {
                        new() {
                            FieldName = "Name",
                            ChainingPredicate = "AND",
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
                    ChainingPredicate = "AND",
                    IndexedFields = new List<IndexedFieldConfiguration>
                    {
                        new() {
                            FieldName = "Name",
                            ChainingPredicate = "AND",
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
                            ChainingPredicate = "AND",
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
