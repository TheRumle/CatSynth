using Common.String;

namespace Cat.Verify.Definitions;

public sealed record Arrival(IEnumerable<string> ProductIds, string Machine): IProductInvolvedOperation, IMachineOperation
{
    /// <inheritdoc />
    public string ToPaperFormattedString()
    {
        return $"Arrive({ProductIds.CommaSeparate()}, {Machine})";
    }
}