using ModelChecker;

namespace CommandLineApp;

public static class InputAssertion
{
    public static bool ExamineIfGuidedOrUnguidedSearch(
        string algorithmName,
        string? heuristicName, HeuristicCollection heuristicCollection,
        SynthesizerCollection synthesizerCollection)
    {
        
        if (!synthesizerCollection.AllGuidedKeys.Union(synthesizerCollection.AllUnguidedKeys).Contains(algorithmName))
            throw new Exception($"No such synthesizer alg. {algorithmName}");
    
        bool isGuidedSearch = synthesizerCollection.AllGuidedKeys.Contains(algorithmName);
        bool heuristicGiven = heuristicName is not null;
        if (heuristicGiven && !heuristicCollection.HeuristicNames.Contains(heuristicName))
            throw new Exception("No heuristic called " + heuristicName);
    
        if (!heuristicGiven && isGuidedSearch)
            throw new Exception("A heuristic must be provided for guided algorithm");

        if (heuristicGiven && !isGuidedSearch)
            throw new Exception("Cannot use heuristic for unguided algorithm.");
    
    
 

        return isGuidedSearch;
    }
}