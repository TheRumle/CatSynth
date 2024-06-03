namespace ModelChecker.Domain.Actions;

public abstract record SystemAction
{
    /// <inheritdoc />
    public abstract string ActionName();
};