namespace Scheduler.ModelChecker.IntegrationTest;

public static class EnumerableExtensions
{
    public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) where T : notnull
    {
        return enumerable.Select((e, i) => predicate.Invoke(e) ? i : -1).Where(i => i != -1);
    }
}