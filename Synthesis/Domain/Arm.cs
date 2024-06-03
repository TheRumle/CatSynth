namespace ModelChecker.Domain;

public sealed record Exit: Location
{

    /// <inheritdoc />
    public Exit(string name) : base(int.MaxValue, name)
    {
    }
}

public sealed record Arm : Location
{
    /// <inheritdoc />
    public bool Equals(Arm? other)
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

    public readonly Machine[] ReachableMachines;
    public readonly Exit[] ReachableExits;
    public readonly int Time;
    public Arm(string armName, Machine[] reachableMachines, int capacity, int time, Exit[] reachableExits) : base(capacity, armName)
    {
        ReachableMachines = reachableMachines;
        Time = time;
        ReachableExits = reachableExits;
    }

    public bool CanReach(Machine machineId)
    {
        return ReachableMachines.Contains(machineId);
    }
    public bool CanReach(Exit machineId)
    {
        return ReachableExits.Contains(machineId);
    }
    


    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name}";
    }
}