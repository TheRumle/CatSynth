namespace ModelChecker.Domain.Actions;

public sealed record Delay(int Amount) : SystemAction
{
    /// <inheritdoc />
    public override string ActionName()
    {
        return $"Wait {Amount}";
    }
}