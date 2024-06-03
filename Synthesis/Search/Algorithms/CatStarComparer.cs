using ModelChecker.Domain;

namespace ModelChecker.Search.Algorithms;

internal sealed class CatStarComparer(Dictionary<Configuration, (string where, float g, float f, float h)> where) : IComparer<ReachableConfiguration>
{
    public int Compare(ReachableConfiguration x, ReachableConfiguration y)
    {
        var compared = where[x.ReachedConfiguration].f.CompareTo(where[y.ReachedConfiguration].f);
        if (x.ReachedConfiguration.Equals(y.ReachedConfiguration))
            return 0;

        if (compared == 0)
            return 1;
        return compared;
    }
}