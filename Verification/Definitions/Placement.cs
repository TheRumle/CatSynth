using Common.String;

namespace Cat.Verify.Definitions;

public sealed record Placement(IEnumerable<string> ProductIds, string Machine, string Arm): IMachineOperation, IArmOperation, IProductInvolvedOperation
{
    /// <inheritdoc />
    public string ToPaperFormattedString()
    {
        return $"place({Arm}, {{{ProductIds.CommaSeparate()}}}, {Machine})";
    }
}