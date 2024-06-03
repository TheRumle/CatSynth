namespace ModelChecker.Domain.Actions;

public sealed record ExitPlacement(Arm Arm, Exit E): SystemAction, IArmOperation
{
    /// <inheritdoc />
    public override string ActionName()
    {
        return$"'{Arm.Name}' places all it holds in exit";
    }
}