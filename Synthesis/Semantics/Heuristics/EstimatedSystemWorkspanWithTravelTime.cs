using ModelChecker.Domain;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics.Heuristics.Utility;

namespace ModelChecker.Semantics.Heuristics;

public class EstimatedSystemWorkspanWithTravelTime : ISearchHeuristic
{
    private TravelTimeCalculator _travelTimeCalculator = null!;
    private RemainingWorkspanCalculator _workspanCalculator = null!;

    /// <inheritdoc />
    public float CalculateCost(Configuration configuration)
    {
        return _travelTimeCalculator.TravelTimeForForInputAndSystem(configuration) +
               _workspanCalculator.MinWorkspanForNotYetInputtedProducts(configuration).Sum() + 
               _workspanCalculator.MinimumWorkspanForProducts(configuration).Sum();
    }

    /// <inheritdoc />
    public void Initialize(SchedulingProblem problem)
    {
        _travelTimeCalculator = new TravelTimeCalculator(problem.StartConfig, problem.InputSequence, problem.Protocols);
        _workspanCalculator = new(problem.InputSequence, problem.Protocols);
    }

    /// <inheritdoc />
    public string PaperName { get; } = "RPT-T";
}