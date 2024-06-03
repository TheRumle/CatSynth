using Common;
using Common.Results;
using ModelChecker.Domain;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics.Heuristics;

namespace ModelChecker;

public class HeuristicCollection
{
    public static readonly ISearchHeuristic NoHeuristic = new NoHeuristicFunction();
    private class NoHeuristicFunction: ISearchHeuristic
    {
        /// <inheritdoc />
        public float CalculateCost(Configuration configuration)
        {
            return int.MaxValue;
        }

        /// <inheritdoc />
        public void Initialize(SchedulingProblem problem)
        {
        }

        /// <inheritdoc />
        public string PaperName { get; } = "No heuristic";
    } 
    
    
    private readonly IEnumerable<(ISearchHeuristic heuristic, string description)> _collection = [
    
       (new EstimatedSystemWorkspan(), "The estimated remaining workspan for the entire system. Assumes machines run at full capacity. "),
        (new EstimatedSystemWorkspanWithTravelTime(), "The estimated remaining workspan for the entire system, accounts for arm move times with arm running at full capacity."),
        (new MaxRemainingWorkSpan(), "The maximum remaining processing time of all products."),
        (new MaxRemainingWorkspanWithTravel(), "The maximum remaining processing time of all products while accounting for the time it takes to traverse remaining protocol.")
    ];
    
    private IEnumerable<(string name, string decription)> NamesAndDescriptions =>
        _collection.Select(e=>(name: e.heuristic.PaperName, e.description));

    public IEnumerable<string> HeuristicNames => _collection.Select(e => e.heuristic.PaperName);
    public IEnumerable<string> HeuristicNameDescriptions => _collection.Select(e => $"{e.heuristic.PaperName} - {e.description}");


    public IEnumerable<ISearchHeuristic> All()
    {
        return _collection.Select(e=>e.heuristic);
    }
    
    public Result<ISearchHeuristic> GetResult(string name)
    {
        try
        {
            return Result.Success(_collection.First(e=>e.heuristic.PaperName.ToLower().Equals(name.ToLower())).heuristic);
        }
        catch (Exception)
        {
            return Result.Failure<ISearchHeuristic>(new Error("SearchHeuristicMissing", $"No search heuristic called {name}"));
        }
    }
}