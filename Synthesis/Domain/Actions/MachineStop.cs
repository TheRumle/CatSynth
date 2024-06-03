namespace ModelChecker.Domain.Actions;

public sealed record MachineStop : SystemAction, IMachineAction
{

    public MachineStop(Machine machine)
    {
        Machine = machine;
    }

    /// <inheritdoc />
    public override string ActionName()
    {
        return $"stop '{Machine.Name}'";
    }

    /// <inheritdoc />
    public Machine Machine { get; }
}