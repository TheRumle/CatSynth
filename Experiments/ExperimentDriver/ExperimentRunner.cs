using System.Diagnostics;
using System.Text;
using Cat.Verify;
using CatConversion.SynthesisVerification;
using Common;
using Common.Results;
using ModelChecker;
using ModelChecker.Problem;
using ModelChecker.Search;

namespace Experiments.ExperimentDriver;

public abstract class Runner(ScheduleCatVerifier _verifier)
{
    protected ScheduleCatVerifier Verifier = _verifier;
    protected async Task<ExperimentResultContext> Execute(ICatSynthesiser algorithm,ISearchHeuristic heuristic,  TimeSpan timeout, Stopwatch stopwatch, SchedulingProblem problem)
    {
        try
        {
            stopwatch.Restart();
            var scheduleResult = await algorithm.ExecuteAsync(timeout);
            var experimentResult = await ConstructExperimentResult(stopwatch, scheduleResult, algorithm,heuristic,0, problem);

            return experimentResult;
        }
        catch (OutOfMemoryException e)
        {
            stopwatch.Stop();
            return new ExperimentResultContext(ExperimentResult.NoSchedule(problem, algorithm, heuristic, stopwatch.Elapsed, -1),
                [new Error("OUT OF MEMORY - FATAL", "The runtime ran out of memory")],null,null);
        }
    }
    
    private async Task<ExperimentResultContext> ConstructExperimentResult(Stopwatch stopwatch, Result<Schedule> result, ICatSynthesiser algorithm,
        ISearchHeuristic searchHeuristic, long virtualMemoryUsedBt, SchedulingProblem problem)
    {
        var elapsed = stopwatch.Elapsed;
        var kb = virtualMemoryUsedBt / 1000;
        
        return result switch
        {
            { IsSuccess: true } => await CreateSuccessContext(algorithm, searchHeuristic, problem, elapsed,kb, result.Value),
            { IsSuccess: false } => CreateFailureContext(algorithm, searchHeuristic, problem, elapsed,kb)
        };
    }

    private async Task<ExperimentResultContext> CreateSuccessContext(ICatSynthesiser algorithm,
        ISearchHeuristic searchHeuristic, SchedulingProblem problem, TimeSpan elapsed, double kb, Schedule schedule)
    {
        var (context, execution) = ScheduleCatConverter.Convert(problem, schedule);
        
        var verification = await Verifier.Verify(context, execution);
        var result = ExperimentResult.Successful(problem, algorithm, searchHeuristic, elapsed, kb, schedule, verification.Errors.Any());
        return new ExperimentResultContext(result, verification.Errors, schedule, execution);
    }
    
    private ExperimentResultContext CreateFailureContext(ICatSynthesiser algorithm,
        ISearchHeuristic searchHeuristic, SchedulingProblem problem, TimeSpan elapsed, double kb)
    {
        var result = ExperimentResult.NoSchedule(problem, algorithm, searchHeuristic, elapsed, kb);
        return new ExperimentResultContext(result, [], null, null);
    }
    
   
}

public record ExperimentResultContext(ExperimentResult Result, Error[] VerificationErrors, Schedule? Schedule, Execution? Execution){}

public sealed class ExperimentRunner : Runner
{
    private readonly SynthesiserFactory _synthesiserFactory;
    
    

    public ExperimentRunner(SynthesiserFactory synthesiserFactory, ScheduleCatVerifier verifier):base(verifier)
    {
        _synthesiserFactory = synthesiserFactory;
    }


    public async Task<ExperimentResultContext> RunDepthFirst(SchedulingProblem problem, TimeSpan timeout)
    {
        var stopwatch = new Stopwatch();
        CollectGarbage();
        using var algorithm = _synthesiserFactory.DepthFirst(problem);
        return await Execute(algorithm, HeuristicCollection.NoHeuristic, timeout, stopwatch, problem);
    }
    
    
    public async Task<ExperimentResultContext> RunUnguidedSynthesizer(ICatSynthesiser synthesiser, TimeSpan timeout, SchedulingProblem problem)
    {
        var stopwatch = new Stopwatch();
        CollectGarbage();
        return await Execute(synthesiser, HeuristicCollection.NoHeuristic, timeout, stopwatch, problem);
    }

    
    
    
    private async Task<ExperimentResultContext> ExecuteCatStar(ISearchHeuristic searchHeuristic, Stopwatch stopwatch,
        SchedulingProblem problem, TimeSpan timeout)
    {
        using var algorithm = _synthesiserFactory.CatStar(searchHeuristic, problem);
        return await Execute(algorithm, searchHeuristic, timeout, stopwatch, problem);
    }

    
    

    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    

    public async Task<ExperimentResultContext> RunHeuristicSynthesizer(ICatSynthesiser synthesiser, TimeSpan timeOut, SchedulingProblem schedulingProblem, ISearchHeuristic heuristic)
    {
        var stopwatch = new Stopwatch();
        CollectGarbage();
        return await Execute(synthesiser, heuristic, timeOut, stopwatch, schedulingProblem);
    }
}