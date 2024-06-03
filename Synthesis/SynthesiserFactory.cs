using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Search.Algorithms;
using ModelChecker.Semantics;

namespace ModelChecker;

public class SynthesiserFactory
{
    public ICatSynthesiser CatStar(ISearchHeuristic heuristic, SchedulingProblem problem)
    {
        var actionExecutor = new ActionExecutor(problem.DeadlineCollection, problem.InputSequence);
        var configurationExplorer =
            new ConfigurationExplorer(new PossibleDelayComputer(problem.DeadlineCollection, problem.InputSequence),
                actionExecutor);

        return new CatStar(heuristic, configurationExplorer, problem);
    }
    
    public ICatSynthesiser DepthFirst(SchedulingProblem problem)
    {
        var actionExecutor = new ActionExecutor(problem.DeadlineCollection, problem.InputSequence);
        var configurationExplorer =
            new ConfigurationExplorer(new PossibleDelayComputer(problem.DeadlineCollection, problem.InputSequence),
                actionExecutor);

        return new DepthFirst(problem, configurationExplorer);
    }
}