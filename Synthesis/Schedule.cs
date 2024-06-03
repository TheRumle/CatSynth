using ModelChecker.Domain;
using ModelChecker.Domain.Actions;

namespace ModelChecker;

public record struct ScheduleStep(Configuration from, SystemAction action, Configuration to)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{from} <{action.ActionName()}> {to}";
    }
}

public sealed class Schedule
{
    public readonly int TotalMakespan;

    public Schedule(IEnumerable<ReachableConfiguration> reachedStates, int totalMakespan)
    {
        var reachableConfigurations =  reachedStates.OrderBy(e=>e.ReachedConfiguration.Time).ToArray();
        TotalMakespan = totalMakespan;
        ReachedStates = reachableConfigurations.ToList();
    }

    /// <summary>
    /// Ordered by configuration time
    /// </summary>
    public readonly List<ReachableConfiguration> ReachedStates;

    /// <inheritdoc />
    public override string ToString()
    {
        var actionPerTime = ReachedStates
            .Where(e => e.ReachedBy is not Delay)
            .GroupBy(e => e.ReachedConfiguration.Time)
            .OrderBy(e => e.Key)
            .Select(e => $"{e.Key}: [{string.Join(", ", e.Select(reachable=>reachable.ReachedBy.ActionName()))}]");


        return string.Join("\n", actionPerTime);
    }
    
    public IEnumerable<ScheduleStep> ToTimeline()
    {
        
        for (int i = 1; i < ReachedStates.Count; i++)
        {
            var current = ReachedStates[i];
            yield return new ScheduleStep(ReachedStates[i - 1].ReachedConfiguration, current.ReachedBy, current.ReachedConfiguration);
        }
    }

    public string ToTimelineString()
    {
        return string.Join("--", ToTimeline().Select((step,i) => $"({step.from.Time}, {step.action.ActionName()})--> C_{i+1} "));
    }
}