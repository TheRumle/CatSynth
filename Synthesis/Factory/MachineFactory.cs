using ModelChecker.Domain;

namespace ModelChecker.Factory;

public class MachineFactory
{
    public Machine CreateMachine(int capacity, string machineName, IEnumerable<int> requiredParts, MachineState state)
    {
        return new Machine(capacity, machineName, requiredParts.ToArray(), state);
    }
}