using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;

namespace ModelChecker.Search.Algorithms;



internal sealed class CatStarDepthFirst : HeuristicCatSynthesizer
{
    private readonly Dictionary<Configuration, int> _depths = new();
    private List<(ReachableConfiguration config, float f, int depth)> openDepths;
    private float _currentThreshold;

    public CatStarDepthFirst(ISearchHeuristic heuristic,
        IConfigurationExplorer configurationExplorer,
        SchedulingProblem schedulingProblem, float epsilon, string epsilonString) : base(heuristic, schedulingProblem, configurationExplorer)
    {
        _epsilon = epsilon;
        _epsilonStr = epsilonString;
    }


    private readonly float _epsilon;
    private readonly string _epsilonStr;
    public override string Name =>$"Cat-DFS-{_epsilonStr}";

    protected override void AdjustOpenAndClosed(IEnumerable<ReachableConfiguration> reachableStates, ReachableConfiguration current)
    {
        var currentDepth = _depths[current.ReachedConfiguration];
        foreach (var next in reachableStates)
        {
            var neighbor = next.ReachedConfiguration;

            var isSeenBefore = Where.TryGetValue(neighbor, out var prevState);
            if (!isSeenBefore)
            {
                AddOpen(next);
                PreviousFor[next] = current;
                _depths[neighbor] = currentDepth + 1;
                continue;
            }


            if (prevState.where == "closed" && prevState.g > neighbor.Time)
            {
                UpdateOpen(next, prevState.h, next.ReachedConfiguration.Time);
                PreviousFor[next] = current;
                _depths[neighbor] = currentDepth + 1;
                continue;
            }

            if (prevState.where != "open" || !(prevState.g > neighbor.Time)) continue;
            
            UpdateOpen(next, prevState.h, next.ReachedConfiguration.Time);
            PreviousFor[next] = current;
            _depths[neighbor] = currentDepth + 1;
        }
    }


    protected override void InitializeFirstMember()
    {
        var first = ReachableConfiguration.FromAction(new Delay(0), SchedulingProblem.StartConfig);
        _open[first] = 0;
        Where[first.ReachedConfiguration] = ("open", 0, 0, Heuristic.CalculateCost(first.ReachedConfiguration));
        _depths[first.ReachedConfiguration] = 0;
        
    }
    
    protected override ReachableConfiguration Pop()
    {
        var current = _open.First();
        _currentThreshold = current.Value * (1 + _epsilon);
        
        var best = _open
            .TakeWhile(LessThanThreshold)
            .MaxBy(Deepest);

        _open.Remove(best.Key);

        return best.Key;
    }

    private bool LessThanThreshold((ReachableConfiguration conf, float f, int depth) tuple)
    {
        return tuple.f <= _currentThreshold;
    }
 

    private bool LessThanThreshold(KeyValuePair<ReachableConfiguration, float> kvp)
    {
        return kvp.Value <= _currentThreshold;
    }

    private int Deepest(KeyValuePair<ReachableConfiguration, float> kvp)
    {
        return _depths[kvp.Key.ReachedConfiguration];
    }
    
    private int Deepest((ReachableConfiguration conf, float f, int depth) tuple)
    {
        return tuple.depth;
    }

    

    /// <inheritdoc />
    public new void Dispose()
    {
        _open.Clear();
        base.Dispose();
    }
}