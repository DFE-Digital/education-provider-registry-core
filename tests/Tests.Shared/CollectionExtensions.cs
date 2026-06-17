namespace Tests.Shared;

public static class CollectionExtensions
{
    public static bool CollectionsMatch<TInstance>(
        this IEnumerable<TInstance> actual,
        IEnumerable<TInstance> expected) =>
            expected
                .Zip(actual, (expected, actual) => Equals(expected, actual))
                .All(match => match);

    public static bool CollectionsMatch<TInstance>(
        this IEnumerable<TInstance> actual,
        IEnumerable<TInstance> expected,
        Func<TInstance, TInstance, bool> predicate) =>
            expected
                .Zip(actual, (expected, actual) => predicate(expected, actual))
                .All(match => match);
}
