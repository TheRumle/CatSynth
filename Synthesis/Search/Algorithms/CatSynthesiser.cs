using Common.Results;
using ModelChecker.Problem;
using ModelChecker.Search.SearchErrors;

namespace ModelChecker.Search.Algorithms;

internal abstract class CatSynthesiser(SchedulingProblem schedulingProblem, IConfigurationExplorer configurationExplorer) 
    : ICatSynthesiser
{
    /// <inheritdoc />
    public abstract string Name { get; }
    public int NumberOfConfigurationsExplored { get; protected set; }
    protected readonly Dictionary<ReachableConfiguration, ReachableConfiguration> PreviousFor = new();


    /// <inheritdoc />
    public  Result<Schedule> Execute(TimeSpan timeOut)
    {
        var tokenSource =  new CancellationTokenSource();
        tokenSource.CancelAfter(timeOut);
        ReachableConfiguration? goal = Search(tokenSource.Token);
        return ExtractResult(goal);
    }

    /// <inheritdoc />
    public Task<Result<Schedule>> ExecuteAsync(TimeSpan timeOut)
    {
        return Task.Run(() => Execute(timeOut));
    }
    /// <inheritdoc />
    public void Dispose()
    {
        
    }
    
    protected readonly SchedulingProblem SchedulingProblem = schedulingProblem;
    protected readonly IConfigurationExplorer ConfigurationExplorer = configurationExplorer;
    protected readonly InputSequence InputSequence = schedulingProblem.InputSequence;

    public Schedule ConstructSchedule(ReachableConfiguration goal)
    {
        var reachedConfigurations = CreateReachedStates(goal);
        return new Schedule(reachedConfigurations, goal.ReachedConfiguration.Time);
    }
    

    /// <inheritdoc />
    protected abstract ReachableConfiguration? Search(CancellationToken token);
    private Result<Schedule> ExtractResult(ReachableConfiguration? goal)
    {
        return goal is null
            ? Result.Failure<Schedule>(Errors.CouldNotFindSolution(SchedulingProblem.SystemName, NumberOfConfigurationsExplored))
            : Result.Success(ConstructSchedule(goal));
    }
    
    
    /// <summary>
    /// Creates a stack of reached configurations. The top of the stack is the initial configuration the bottom is the goal configuration.
    /// </summary>
    /// <param name="goal"></param>
    /// <returns>A stack of reached configurations. Top is the initial, last is the goal.</returns>
    private Stack<ReachableConfiguration> CreateReachedStates(ReachableConfiguration goal)
    {
        var result = new Stack<ReachableConfiguration>();
        result.Push(goal);
        var prev = goal;
        do
        {
            prev = PreviousFor[prev];
            result.Push(prev);
        } while (!prev.ReachedConfiguration.Equals(SchedulingProblem.StartConfig));
        return result;
    }

}