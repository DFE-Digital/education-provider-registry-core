//using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
//using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
//using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
//using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

//namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.Request;

//public class SearchRequestTests
//{
//    [Fact]
//    public void Constructor_WithFilterParam_PopulatesFilterRequests()
//    {
//        // arrange
//        List<FilterRequest> filterRequests = [FilterRequestTestDouble.Fake()];
//        SortOrder sortOrder = SortOrderTestDouble.Stub();

//        // act
//        SearchRequest request = new(
//            searchIndexKey: "stubIndexKey",
//            searchKeywords: "searchKeyword",
//            filterRequests: filterRequests,
//            sortOrder: sortOrder);

//        // assert
//        Assert.NotNull(request.FilterRequests);
//        Assert.NotNull(request.SortOrder);

//        FilterRequest expected = filterRequests[0];

//        FilterRequest? actual =
//            request.FilterRequests!
//                .FirstOrDefault(fr => fr.FilterName == expected.FilterName);

//        Assert.NotNull(actual);
//        Assert.Equal(expected.FilterValues.Count, actual!.FilterValues.Count);

//        Assert.True(actual.FilterValues.CollectionsMatch(
//            expected.FilterValues,
//            (expected, actual) => Equals(expected, actual)));
//    }

//    [Fact]
//    public void Constructor_WithNoFilterParam_HasFilterRequestsNull()
//    {
//        // arrange
//        SortOrder sortOrder = SortOrderTestDouble.Stub();

//        // act
//        SearchRequest request =
//            new(
//                searchIndexKey: "stubIndexKey",
//                searchKeywords: "searchKeyword",
//                sortOrder: sortOrder);

//        // assert
//        Assert.NotNull(request.SortOrder);
//        Assert.Null(request.FilterRequests);
//    }

//    [Fact]
//    public void Constructor_WithSetOffsetValue_AssignsCorrectPropertyValue()
//    {
//        // arrange
//        SortOrder sortOrder = SortOrderTestDouble.Stub();
//        const int Offset = 10;

//        // act
//        SearchRequest request =
//            new(
//                searchIndexKey: "stubIndexKey",
//                searchKeywords: "searchKeyword",
//                sortOrder: sortOrder,
//                offset: Offset);

//        // assert
//        Assert.NotNull(request.SortOrder);
//        Assert.Equal(Offset, request.Offset);
//    }

//    [Fact]
//    public void Constructor_WithDefaultOffsetValue_AssignsDefaultPropertyValue()
//    {
//        // arrange
//        SortOrder sortOrder = SortOrderTestDouble.Stub();

//        // act
//        SearchRequest request =
//            new(
//                searchIndexKey: "stubIndexKey",
//                searchKeywords: "searchKeyword",
//                sortOrder: sortOrder);

//        // assert
//        Assert.NotNull(request.SortOrder);
//        Assert.Equal(0, request.Offset); // default means no records skipped
//    }
//}
