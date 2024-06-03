namespace Cat.Verify.Definitions;

public sealed class CatContext
{
    public required IEnumerable<string> ProductIds { get; init; }
    public required IEnumerable<string> Exits { get; init; }
    public required IEnumerable<string> ActiveMachines { get; init; }
    public required IEnumerable<string> Machines { get; init; }
    public required IEnumerable<string> Arms { get; init; }
    public required IReadOnlyDictionary<string, int> ArmTimes { get; init; }
    public required IReadOnlyDictionary<string, IEnumerable<string>> ArmReach { get; init; }
    public required IReadOnlyDictionary<string, int> Capacity { get; init; }
    public required IReadOnlyDictionary<string, IEnumerable<int>> RequiredParts { get; init; }
    public required IReadOnlyDictionary<string, IEnumerable<(string location, int minTime, int maxTime)>> Protocol { get; init; }
    public required IReadOnlyDictionary<string, (int startStep, int endStep, int deadline)> CriticalSection { get; init; }

    public const string BOT = "BOT";
    
}



