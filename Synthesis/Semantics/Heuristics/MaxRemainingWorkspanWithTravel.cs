using ModelChecker.Domain;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics.Heuristics.Utility;

namespace ModelChecker.Semantics.Heuristics;

public sealed class MaxRemainingWorkspanWithTravel : ISearchHeuristic
{
    private RemainingWorkspanCalculator _workspanCalculator = null!;
    private TravelTimeCalculator _travelTimeCalculator = null!;
    private ProtocolList _prot;
    private InputSequence _seq;

    /// <inheritdoc />
    public float CalculateCost(Configuration configuration)
    {
        var workspansForNotInputted =
            _seq.InputSequenceAtTime(configuration.Time)
                .Select(_prot.ProtocolFor)
                .Select(protocol =>
                    protocol.AllSteps.Sum(e => e.MinProcessingTime) 
                    + _travelTimeCalculator.TimeToTraverseProtocol(protocol.AllSteps))
                .DefaultIfEmpty(0).Max();
        
        
        var workspanForInputted = configuration.PartsByLocation
            .SelectMany(locationParts =>
                locationParts.Value.Select(e =>
                    _workspanCalculator.MinWorkspanForProduct(e) + _travelTimeCalculator.TimeToTraverseProtocol(e.RemainingProtocol)))
            .DefaultIfEmpty(0).Max();

        return Math.Max(workspanForInputted, workspansForNotInputted);

    }

    /// <inheritdoc />
    public void Initialize(SchedulingProblem problem)
    {
        _travelTimeCalculator = new TravelTimeCalculator(problem.StartConfig, problem.InputSequence, problem.Protocols);
        _workspanCalculator = new RemainingWorkspanCalculator(problem.InputSequence, problem.Protocols);
        _seq = problem.InputSequence;
        _prot = problem.Protocols;
    }

    /// <inheritdoc />
    public string PaperName { get; } = "MRPT-T";
}