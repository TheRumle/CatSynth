using System.Diagnostics.Contracts;

namespace Common.Math.Discrete.Graph;

public class ShortestDistance<T>(Dictionary<(T from, T to), float> distances)
    where T : notnull
{
    public IReadOnlyDictionary<(T from, T to), float> Distances = distances;

    [Pure]
    public float LowestCost(T from, T to)
    {
        return Distances[(from, to)];
    }
}