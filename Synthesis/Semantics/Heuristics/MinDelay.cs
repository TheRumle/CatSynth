using ModelChecker.Domain;
using ModelChecker.Problem;
using ModelChecker.Search;

namespace ModelChecker.Semantics.Heuristics;

public class MinDelay : ISearchHeuristic
{
    /// <inheritdoc />
    public float CalculateCost(Configuration configuration)
    {
        return 0;
    }

    /// <inheritdoc />
    public void Initialize(SchedulingProblem problem)
    {
        //NOOP
    }

    /// <inheritdoc />
    public string PaperName { get; } = "MinDelay";
}