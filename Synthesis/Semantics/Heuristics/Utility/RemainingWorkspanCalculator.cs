using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Semantics.Heuristics.Utility;

public sealed class RemainingWorkspanCalculator(InputSequence inputSequence, ProtocolList protocol)
{
    public IEnumerable<int> MinWorkspanForNotYetInputtedProducts(Configuration configuration)
    {
        return inputSequence
            .InputSequenceAtTime(configuration.Time)
            .Select(protocol.ProtocolFor).Select(e => e.AllSteps.Sum(step => step.MinProcessingTime));
    }

    public IEnumerable<int> MinimumWorkspanForProducts(Configuration configuration)
    {
        return configuration.AllParts.Select(MinWorkspanForProduct);
    }
    
    
    public int MinWorkspanForProduct(Product product)
    {
        return product
            .RemainingProtocol
            .Select(step => step.MinProcessingTime)
            .Sum();
    }
    
}