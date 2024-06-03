namespace ModelChecker.Domain.Actions;

public sealed record Placement(Machine Machine, Arm Arm, IEnumerable<Product> Parts)
    : SystemAction, IMachineAction, IArmOperation
{
    
    /// <inheritdoc />
    public override string ActionName()
    {
        return $"'{Arm.Name}' places {Parts.Aggregate("[", (s, part) => s + " " + part.Id)} ] in '{Machine.Name}'";
    }
}