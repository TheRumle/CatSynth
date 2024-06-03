using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Search.Algorithms;

internal abstract class HeuristicCatSynthesizer : CatSynthesiser
{
    protected readonly ISearchHeuristic Heuristic;
    protected readonly Dictionary<Configuration, (string where, float g, float f, float h)> Where = new ();
    protected readonly SortedList<ReachableConfiguration, float> _open;



    protected HeuristicCatSynthesizer(ISearchHeuristic heuristic, SchedulingProblem schedulingProblem, IConfigurationExplorer configurationExplorer) : base(schedulingProblem, configurationExplorer)
    {
        Heuristic = heuristic;
        Heuristic.Initialize(schedulingProblem);
        _open = new(new CatStarComparer(Where));
    }
    
    /// <inheritdoc />
    public new void Dispose()
    {
        base.Dispose();
    }
    
    protected void Close(Configuration current, (string where, float gValue, float fValue, float hValue ) oldConfig)
    {
        Where[current] = ("closed", oldConfig.gValue, oldConfig.fValue, oldConfig.hValue);
        NumberOfConfigurationsExplored += 1;
    }
    
    protected void UpdateOpen(ReachableConfiguration newCheapest, float hValue, float timeValue)
    {
        var tuple = (gValue: timeValue, fValue: newCheapest.ReachedConfiguration.Time + hValue, hValue);
        Where[newCheapest.ReachedConfiguration] = ("open", timeValue, hValue + timeValue, tuple.hValue);
        _open[newCheapest] = tuple.fValue;
    }
    
    protected void AddOpen(ReachableConfiguration configuration)
    {
        var hValue = Heuristic.CalculateCost(configuration.ReachedConfiguration);
        var tuple = (gValue: configuration.ReachedConfiguration.Time, fValue: configuration.ReachedConfiguration.Time + hValue, hValue);
        Where[configuration.ReachedConfiguration] = ("open", tuple.gValue, tuple.fValue, tuple.hValue);
        _open[configuration] = tuple.fValue;
    }
    
    protected override ReachableConfiguration? Search(CancellationToken token)
    {
        InitializeFirstMember();
        while (_open.Count > 0)
        {
            if (token.IsCancellationRequested) return null;
            
            var current = Pop();
            
            if (current.ReachedConfiguration.IsGoalConfiguration(SchedulingProblem))  
                return current;
            
            var reachableStates = ConfigurationExplorer.GenerateConfigurations(current.ReachedConfiguration);
            
            AdjustOpenAndClosed(reachableStates, current);
            Close(current.ReachedConfiguration, Where[current.ReachedConfiguration]);
        }
        return null;
    }

    protected abstract void AdjustOpenAndClosed(IEnumerable<ReachableConfiguration> reachableStates,
        ReachableConfiguration current);

    protected abstract void InitializeFirstMember();
    protected abstract ReachableConfiguration Pop();

}