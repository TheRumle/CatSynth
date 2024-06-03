using ModelChecker.Domain;

namespace ModelChecker.Problem;

public sealed record SchedulingProblem(
    string SystemName,
    Configuration StartConfig,
    DeadlineCollection DeadlineCollection,
    InputSequence InputSequence,
    ProtocolList Protocols)
{
}