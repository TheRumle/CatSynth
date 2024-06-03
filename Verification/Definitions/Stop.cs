namespace Cat.Verify.Definitions;

public sealed record Stop(string Machine): IMachineOperation
{
    /// <inheritdoc />
    public string ToPaperFormattedString()
    {
        return $"stop({Machine})";
    }
}