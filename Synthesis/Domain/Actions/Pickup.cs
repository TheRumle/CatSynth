namespace ModelChecker.Domain.Actions;

public sealed record Pickup(Machine Machine, Arm Arm, IEnumerable<Product> Parts)
    : SystemAction, IMachineAction, IArmOperation
{
    /// <inheritdoc />
    public override string ActionName()
    {
        return $"'{Arm.Name}' picks up [ { string.Join(",", Parts.Select(e=>e.Id))} ] from '{Machine.Name}'";
    }
}