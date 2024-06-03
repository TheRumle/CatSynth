
namespace ModelChecker.Domain;

[Flags]
public enum MachineState
{
    Off = 0x0001,
    On = Off << 1,
    Active = On << 1
}

public record Machine : Location
{
    /// <inheritdoc />
    public virtual bool Equals(Machine? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
    
    public Machine StoppedVersion()
    {
        return new Machine(Capacity, Name, RequiredAmounts, MachineState.Off);
    }
    
    public Machine StartedVersion()
    {
        return new Machine(Capacity, Name, RequiredAmounts, MachineState.On);
    }
    
    public int[] RequiredAmounts = [1];
    public readonly MachineState State;


    internal Machine(int capacity, string machineName, MachineState state) : base(capacity, machineName)
    {
        State = state;
    }

    internal Machine(int capacity, string machineName) : this(capacity, machineName, MachineState.Off)
    {
    }

    internal Machine(int capacity, string machineName, int[] amounts, MachineState state) : this(capacity, machineName, state)
    {
        RequiredAmounts = amounts;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name}";
    }

    internal static Machine Active(int capacity, string machineName)
    {
        return new Machine(capacity, machineName, [0], MachineState.Active);
    } 
}