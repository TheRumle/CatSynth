using System.Globalization;
using Common;
using Common.Results;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Search.Algorithms;
using ModelChecker.Semantics;

namespace ModelChecker;

public class SynthesizerCollection
{

    private static readonly IEnumerable<float> epsilons =
    [
        0.05f, 0.1f, 0.15f, 0.25f, 0.40f, 0.50f, 0.75f, 1f, 1.25f, 1.50f, 1.75f, 2f, 2.25f, 2.50f, 2.75f, 3f, 3.50f, 3.25f, 3.75f, 4f, 4.5f,5f
    ];

    private Dictionary<string, Func<ISearchHeuristic, IConfigurationExplorer, SchedulingProblem, ICatSynthesiser>> _guidedSynthesizerFactories;

    public SynthesizerCollection()
    {
        _guidedSynthesizerFactories = EpsilonsToCatStarDepthFirsts();
        _guidedSynthesizerFactories.Add("catstar", (heuristic, explorer, problem) => new CatStar(heuristic, explorer, problem));
    }

    private static Dictionary<string, Func<ISearchHeuristic, IConfigurationExplorer, SchedulingProblem, ICatSynthesiser>> EpsilonsToCatStarDepthFirsts()
    {
        return epsilons
            .ToDictionary<float, string, Func<ISearchHeuristic, IConfigurationExplorer, SchedulingProblem, ICatSynthesiser>>(
                e => ("CatDFS-" + e.ToString(CultureInfo.InvariantCulture)).ToLower(),
                e => (heuristic, explorer, problem) => new CatStarDepthFirst(heuristic, explorer, problem, e, e.ToString(CultureInfo.InvariantCulture))
            );
    }


    private readonly Dictionary<string, Func<IConfigurationExplorer, SchedulingProblem, ICatSynthesiser>> _unguidedSynthesizerFactories =
        new Dictionary<string, Func<IConfigurationExplorer, SchedulingProblem, ICatSynthesiser>>()
        {
            {"depth-first",(explorer, problem) => new DepthFirst(problem, explorer)}
        }.ToDictionary(e=>e.Key.ToLower(), e=>e.Value);
    
    
    
    public Result<ICatSynthesiser> GetSearchSynthesizer(ISearchHeuristic heuristic, SchedulingProblem problem, string synthesizerKey)
    {
        var configurationExplorer = CreateConfigurationExplorer(problem);

        return _guidedSynthesizerFactories.TryGetValue(synthesizerKey.ToLower(), out var factory) 
            ? Result.Success(factory.Invoke(heuristic,configurationExplorer, problem)) 
            : Result.Failure<ICatSynthesiser>(new Error("NoSuchAlgorithm", $"No heuristic synthesizer called {synthesizerKey}"));
    }

    private static ConfigurationExplorer CreateConfigurationExplorer(SchedulingProblem problem)
    {
        var actionExecutor = new ActionExecutor(problem.DeadlineCollection, problem.InputSequence);
        var configurationExplorer =
            new ConfigurationExplorer(new PossibleDelayComputer(problem.DeadlineCollection, problem.InputSequence),
                actionExecutor);
        return configurationExplorer;
    }
    
    public Result<ICatSynthesiser> GetUnguidedSynthesizer(SchedulingProblem problem, string synthesizerKey)
    {
        var configurationExplorer = CreateConfigurationExplorer(problem);

        return _unguidedSynthesizerFactories.TryGetValue(synthesizerKey.ToLower(), out var factory) 
            ? Result.Success(factory.Invoke(configurationExplorer, problem)) 
            : Result.Failure<ICatSynthesiser>(new Error("NoSuchAlgorithm", $"No heuristic synthesizer called {synthesizerKey}"));
    }


    /// <inheritdoc />
    public IEnumerable<string> PossibleUnguidedAlgorithms()
    {
        return _unguidedSynthesizerFactories.Keys;
    }

    /// <inheritdoc />
    public IEnumerable<string> PossibleSearchAlgorithms()
    {
        return _guidedSynthesizerFactories.Keys;
    }


    public Result<ICatSynthesiser> GetGuidedByKey(string key, ISearchHeuristic heuristic, SchedulingProblem problem)
    {
        return GetSearchSynthesizer(heuristic, problem, key);
    }

    public Result<ICatSynthesiser> GetUnguidedByKey(string key, SchedulingProblem problem)
    {
        return GetUnguidedSynthesizer( problem, key.ToLower());
    }
    
    

    public IEnumerable<string> AllUnguidedKeys => PossibleUnguidedAlgorithms();
    public IEnumerable<string> AllGuidedKeys => PossibleSearchAlgorithms();

    private static string RoundFloatToPercentage(float num)
    {
        return Math.Floor(num * 100).ToString(CultureInfo.InvariantCulture);
    }
}