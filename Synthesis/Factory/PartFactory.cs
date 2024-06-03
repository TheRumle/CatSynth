using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Factory;

public class PartFactory
{
    
    private static Random  _random = new();
    public PartFactory(IEnumerable<string> parts)
    {
        PartTypes = parts.ToHashSet();
    }

    public HashSet<string> PartTypes { get; set; }

    public Product NewPartWithProcessingTime(Protocol protocol, string partType, int processingTime)
    {
        return NewPartWithDeadlineTime(protocol, partType, processingTime, 0);
    }


    public Product NewPartWithDeadlineTime(Protocol protocol, string partType, int processingTime, int unstableTime)
    {
        if (!partType.Contains(partType))
            throw new ArgumentException($"{partType} is not in the set of given PartTypes of the factory!");
        return new Product(protocol, processingTime, unstableTime, partType, 0, _random.Next());
    }
}