namespace Common;

public static class ListExtensions
{
    public static IEnumerable<T> Shuffle<T>(this T[] list)
    {
        return list.OrderBy(_ => Random.Shared.Next());
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        return list.OrderBy(_ => Random.Shared.Next());
    }
    public static void Pairwise<T>(this IList<T> list, Action<T, T> action)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            action(list[i], list[i + 1]);
        }
    }

    public static void ForwardTo<T>(this IEnumerable<T> values, Action<T> action)
    {
        foreach (var v in values)
        {
            action.Invoke(v);
        }
    }
}