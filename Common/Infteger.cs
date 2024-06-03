namespace Common;

public class Infteger
{
    public static readonly Infteger PositiveInfinity = new();
    public static readonly Infteger NegativeInfinity = new();

    public static implicit operator int(Infteger value)
    {
        return value == PositiveInfinity ? int.MaxValue : int.MinValue;
    }
}