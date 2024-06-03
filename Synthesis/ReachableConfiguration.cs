using ModelChecker.Domain;
using ModelChecker.Domain.Actions;

namespace ModelChecker;

public class ReachableConfiguration
{
    protected bool Equals(ReachableConfiguration other)
    {
        return ReachedConfiguration.Equals(other.ReachedConfiguration);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ReachableConfiguration)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ReachedConfiguration.GetHashCode();
    }

    public void Deconstruct(out Configuration reachedConfiguration, out SystemAction reachedBy)
    {
        reachedBy = ReachedBy;
        reachedConfiguration = ReachedConfiguration;
    }

    public readonly SystemAction ReachedBy;
    public readonly Configuration ReachedConfiguration;

    private ReachableConfiguration(SystemAction reachedBy, Configuration reachedConfiguration)
    {
        ReachedBy = reachedBy;
        ReachedConfiguration = reachedConfiguration;
    }
    
    public static ReachableConfiguration FromAction(SystemAction reachedBy, Configuration reachedConfiguration)
    {
        return reachedBy switch
        {
            ExitPlacement exit => new ReachableConfiguration(exit, reachedConfiguration),
            MachineStart machineStart => new ReachableConfiguration(machineStart, reachedConfiguration),
            MachineStop machineStop => new ReachableConfiguration(machineStop, reachedConfiguration),
            Pickup pickup => new ReachableConfiguration(pickup, reachedConfiguration),
            Placement placement => new ReachableConfiguration(placement, reachedConfiguration),
            Delay delayAction => OfDelay(delayAction, reachedConfiguration),
            _ => throw new ArgumentException($"UNEXPECTED {reachedBy.GetType().Name} is not a {nameof(SystemAction)}")
        };
    }


    public static ReachableConfiguration OfDelay(Delay delay, Configuration reachedConfiguration)
    {
        return new ReachableConfiguration(delay, reachedConfiguration);
    }
}