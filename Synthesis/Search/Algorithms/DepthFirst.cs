using Common;
using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;

namespace ModelChecker.Search.Algorithms;

internal sealed class DepthFirst(SchedulingProblem schedulingProblem, IConfigurationExplorer configurationExplorer) 
    : CatSynthesiser(schedulingProblem, configurationExplorer)
{
    
    private readonly PriorityQueue<(ReachableConfiguration configuration, int depth), int> _open = new(10000);
    private readonly HashSet<Configuration> _visited = new(10000);

    /// <inheritdoc />
    public override string Name => "DFS";

    protected override ReachableConfiguration? Search(CancellationToken token)
    {
        var start = ReachableConfiguration.OfDelay(new Delay(0), SchedulingProblem.StartConfig);
        _open.Enqueue((start, 0), 0);
            
        while (_open.Count > 0)
        {
            var (currentConfiguration, currentDepth) = _open.Dequeue();
            if (_visited.Contains(currentConfiguration.ReachedConfiguration))
            {
                continue;
            }
            
            if (currentConfiguration.ReachedConfiguration.IsGoalConfiguration(SchedulingProblem))
            {
                return currentConfiguration;
            }
            
            foreach (var neighbor in ConfigurationExplorer.GenerateConfigurations(currentConfiguration.ReachedConfiguration))
            {
                if (_visited.Contains(neighbor.ReachedConfiguration))
                    continue;

                _open.Enqueue((neighbor, currentDepth+1), -(currentDepth-1));
                PreviousFor[neighbor] = currentConfiguration;
            }
            
            _visited.Add(currentConfiguration.ReachedConfiguration);
            NumberOfConfigurationsExplored += 1;
        }

        return null;
    }
    
    /// <inheritdoc />
    public new void Dispose()
    {
        _open.Clear();
        _visited.Clear();
        base.Dispose();
    }


}