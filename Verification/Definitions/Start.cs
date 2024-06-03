namespace Cat.Verify.Definitions;

public sealed record Start(string Machine): IMachineOperation
{
    /// <inheritdoc />
    public string ToPaperFormattedString()
    {
        return $"start({Machine})";
    }
}