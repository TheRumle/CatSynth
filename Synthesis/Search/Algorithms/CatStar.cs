using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;

namespace ModelChecker.Search.Algorithms;

internal sealed class CatStar(
    ISearchHeuristic heuristic,
    IConfigurationExplorer configurationExplorer,
    SchedulingProblem schedulingProblem)
    : HeuristicCatSynthesizer(heuristic, schedulingProblem, configurationExplorer)
{
    protected override void AdjustOpenAndClosed(IEnumerable<ReachableConfiguration> reachableStates, ReachableConfiguration current)
    {
        foreach (var next in reachableStates)
        {
            var neighbor = next.ReachedConfiguration;

            var isSeenBefore = Where.TryGetValue(neighbor, out var prevState);
            if (!isSeenBefore)
            {
                AddOpen(next);
                PreviousFor[next] = current;
                continue;
            }

            if (prevState.where == "closed" && prevState.g > neighbor.Time)
            {
                UpdateOpen(next, prevState.h, next.ReachedConfiguration.Time);
                PreviousFor[next] = current;
                continue;
            }

            if (prevState.where != "open" || !(prevState.g > neighbor.Time)) continue;
            
            UpdateOpen(next, prevState.h, next.ReachedConfiguration.Time);
            PreviousFor[next] = current;
        }
    }

    

    protected override void InitializeFirstMember()
    {
        var first = ReachableConfiguration.FromAction(new Delay(0), SchedulingProblem.StartConfig);
        AddOpen(first);
    }

    /// <inheritdoc />
    public override string Name => "Cat*";
    

    protected override ReachableConfiguration Pop()
    {
        var current = _open.First();
        _open.RemoveAt(0);
        return current.Key;
    }
    
    /// <inheritdoc />
    public new void Dispose()
    {
        _open.Clear();
        base.Dispose();
    }
}