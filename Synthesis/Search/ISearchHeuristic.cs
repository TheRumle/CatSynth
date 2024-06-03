using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Search;

public interface ISearchHeuristic
{
    float CalculateCost(Configuration configuration);

    /// <summary>
    ///     Initialize the search heuristics with the problem s.t internal structures can be constructed only once.
    /// </summary>
    /// <param name="problem"></param>
    public void Initialize(SchedulingProblem problem);


    public string PaperName { get; }
}