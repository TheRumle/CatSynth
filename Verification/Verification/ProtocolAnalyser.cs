using Cat.Verify.Definitions;

namespace Cat.Verify.Verification;

internal sealed class ProtocolAnalyser(CatContext context)
{
    public static (string machine, int minTime, int maxTime) Completed = ("DONE", 0, int.MaxValue);
    public (string machine, int minTime, int maxTime) Next(string productId, Configuration c)
    {
        (string location, int minTime, int maxTime) remaining = context.Protocol[productId].Skip(c[productId].stepsCompleted).FirstOrDefault();
        return remaining == default ? Completed : remaining;
    }
}