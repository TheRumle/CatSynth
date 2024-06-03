using ModelChecker.Domain;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics.Heuristics.Utility;

namespace ModelChecker.Semantics.Heuristics;

public sealed class MaxRemainingWorkSpan : ISearchHeuristic
{
    private RemainingWorkspanCalculator _workspanCalculator = null!;


    public void Initialize(SchedulingProblem problem)
    {
        _workspanCalculator = new RemainingWorkspanCalculator(problem.InputSequence, problem.Protocols);
    }

    /// <inheritdoc />
    public string PaperName { get; } = "MRPT";


    /// <inheritdoc />
    public float CalculateCost(Configuration configuration)
    {
        var workspanForNotInputted = _workspanCalculator.MinWorkspanForNotYetInputtedProducts(configuration).DefaultIfEmpty(0).Max();
        var workspanForInputted    = _workspanCalculator.MinimumWorkspanForProducts(configuration).DefaultIfEmpty(0).Max();
        return Math.Max(workspanForNotInputted, workspanForInputted);
    }
}