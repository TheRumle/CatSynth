using Common;

namespace ModelChecker.Search.SearchErrors;

public static class Errors
{
    public static Error CouldNotFindSolution(string problemName, int numberStates)
    {
        return new Error("NoSolution", $"Could not find solution to {problemName}. Explored {numberStates} configurations");
    }
}