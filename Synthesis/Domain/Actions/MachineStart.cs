namespace ModelChecker.Domain.Actions;

public sealed record MachineStart : SystemAction, IMachineAction
{

    public MachineStart(Machine machine)
    {
        Machine = machine;
    }

    /// <inheritdoc />
    public override string ActionName()
    {
        return $"start '{Machine.Name}'";
    }

    /// <inheritdoc />
    public Machine Machine { get; }
}