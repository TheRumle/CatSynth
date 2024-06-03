using Common.Results;
using ModelChecker;
using ModelChecker.Search;

namespace CommandLineApp;

public class HeuristicAsserter(HeuristicCollection collection)
{
    public  Result<IEnumerable<ISearchHeuristic>> AssertHeuristicsExists(string[] givenHeuristics)
    {
        if (givenHeuristics.Length == 1 && givenHeuristics.First().ToLower() == "all")
        {
            return Result.Success(collection.All());
        }

        return givenHeuristics
            .Select(collection.GetResult)
            .Aggregate();
    }
}