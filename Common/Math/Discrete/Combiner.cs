namespace Common.Math.Discrete;

public static class Combiner
{
    /// <summary>
    ///     Creates lists of all possible combinations of a specific size. Uses regular equality to avoid duplicates.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="size"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ISet<List<T>> AllCombinationsOfSize<T>(IEnumerable<T> elements, int size)
    {
        var result = new HashSet<List<T>>();
        CombineUtil(elements.Distinct().ToList(), size, 0, new List<T>(), result);
        return result;
    }

    private static void CombineUtil<T>(IList<T> elements, int size, int start, List<T> subset, HashSet<List<T>> result)
    {
        //If subset is correct size add it to result and return
        if (subset.Count == size)
        {
            result.Add(new List<T>(subset));
            return;
        }

        for (var i = start; i < elements.Count; i++)
        {
            subset.Add(elements[i]);
            CombineUtil(elements, size, i + 1, subset, result);
            subset.RemoveAt(subset.Count - 1);
        }
    }

    public static IEnumerable<IEnumerable<T>> CartesianProduct<T>
        (this IEnumerable<IEnumerable<T>> sequences)
    {
        IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
        return sequences.Aggregate(emptyProduct,
            (accumulator, sequence) =>
                from previousSequences in accumulator
                from item in sequence
                select previousSequences.Concat(new[] { item }));
    }
}