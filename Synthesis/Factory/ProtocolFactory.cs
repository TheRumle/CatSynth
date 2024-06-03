using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Factory;

public class ProtocolFactory
{


    public ProtocolFactory(IEnumerable<Machine> machines)
    {
        Machines = machines.ToHashSet();
    }

    public HashSet<Machine> Machines { get; set; }


    public Protocol CreateJourney(IEnumerable<(Machine machine, int minProcessingTime, int maxProcessingTime)> steps)
    {
        foreach (var (machine, _, _) in steps)
            if (!Machines.Contains(machine))
                throw new ArgumentException($"{machine} is not part of the available machines of the factory.");

        return new Protocol(steps.Select(e => new ProtocolStep(e.machine, e.minProcessingTime, e.maxProcessingTime)));
    }
}