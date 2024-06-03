using ModelChecker.Domain;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics.Heuristics.Utility;

namespace ModelChecker.Semantics.Heuristics;

public class EstimatedSystemWorkspan : ISearchHeuristic
{
    private RemainingWorkspanCalculator _workspanCalculator = null!;
    

    /// <inheritdoc />
    public float CalculateCost(Configuration configuration)
    {
        return _workspanCalculator.MinWorkspanForNotYetInputtedProducts(configuration).Sum() + 
               _workspanCalculator.MinimumWorkspanForProducts(configuration).Sum();
    }

    /// <inheritdoc />
    public void Initialize(SchedulingProblem problem)
    {
        this._workspanCalculator = new(problem.InputSequence, problem.Protocols);
    }

    /// <inheritdoc />
    public string PaperName => "RPT";
}