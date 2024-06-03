using Common.String;

namespace Cat.Verify.Definitions;

public sealed record Pickup(IEnumerable<string> ProductIds, string Machine, string Arm): IProductInvolvedOperation, IArmOperation, IMachineOperation
{
    /// <inheritdoc />
    public string ToPaperFormattedString()
    {
        return $"take({Arm}, {{ {ProductIds.CommaSeparate()} }}, {Machine})";
    }
}