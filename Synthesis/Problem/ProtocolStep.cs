using ModelChecker.Domain;

namespace ModelChecker.Problem;

public struct ProtocolStep(Machine machine, int minProcessingTime, int maxProcessingTime)
{
    public readonly Machine Machine = machine;
    public readonly int MaxProcessingTime = maxProcessingTime;
    public readonly int MinProcessingTime = minProcessingTime;
}