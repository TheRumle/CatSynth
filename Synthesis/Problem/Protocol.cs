using ModelChecker.Domain;

namespace ModelChecker.Problem;

public sealed class Protocol
{
    public readonly ProtocolStep[] AllSteps;
    public readonly int TotalLength;
    private readonly Dictionary<int, IEnumerable<ProtocolStep>> _allStepSizes = new();

    public Protocol(IEnumerable<ProtocolStep> steps)
    {
        AllSteps = steps.ToArray();
        TotalLength = AllSteps.Length;
        for (int i = 0; i < AllSteps.Length; i++)
        {
            _allStepSizes.Add(i, AllSteps.Skip(i));
        }
        
    }
    
    public Protocol(IEnumerable<(Machine machine, int Min, int Max)> steps): this(steps.Select(e=>new ProtocolStep(e.machine, e.Min,e.Max)))
    {    }


    public IEnumerable<ProtocolStep> SkipSteps(int skipAmount)
    {
        return _allStepSizes.GetValueOrDefault(skipAmount, Enumerable.Empty<ProtocolStep>());
    }
}