using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Search;

public static class ConfigurationExtensions
{
    public static bool IsGoalConfiguration(this Configuration current, SchedulingProblem problem)
    {
        if (current.Time < problem.InputSequence.DoneTime) 
            return false;
        
        var partsByLocation = current.PartsByLocation;
        foreach (var location in current.AllLocations)
        {
            if (location is Exit) 
                continue;
            if (partsByLocation[location].Length != 0)
                return false;
        }

        return true;
    }
}