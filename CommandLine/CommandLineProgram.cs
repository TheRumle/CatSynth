// See https://aka.ms/new-console-template for more information


using Cat.Verify;
using CatConversion.SynthesisVerification;
using Cocona;
using CommandLineApp;
using Common;
using Common.Results;
using Common.String;
using Experiments;
using Experiments.ExperimentDriver;
using ModelChecker;
using ModelChecker.Search;
using Scheduler.Input;
using Scheduler.Input.Json;
using Scheduler.Input.Json.Parse.Synthesis;

var builder = CoconaApp.CreateBuilder();

builder.Services
    .AddCatVerificationServices()
    .AddSynthesisServices()
    .AddScheduleToCatAdaption()
    .AddJsonInputServices()
    .AddExperimentRunnerServices()
    ;

var consoleApp = builder.Build();
builder.Configuration["runtimeOptions:configProperties:System.GC.AllowVeryLargeObjects"] = "true";

consoleApp.AddCommand("synth-dir-guided", async (
    [Argument(Description =
        "The directory containing a .json file describing system and a subdirectory /seqs/ containing sequences the topology of the system.")]
    string dir,
    [Argument]
    string algorithm,
    [Argument(Description = "Which heuristics to use. Use command 'heuristics' to list possible heuristics.")]
    string[] heuristics,
    [Option(Description = $"The timeout in minutes. Default is 60 minutes.")]
    int? timeOut,
    HeuristicCollection collection,
    JsonSynthesisProblemLoader problemLoader,
    SynthesisProblemMapper problemMapper,
    ExperimentRunner experimentRunner,
    SynthesizerCollection synthesizerCollection
) =>
{

    HeuristicAsserter heuristicAsserter = new(collection);
        
    var directoryAssert = ResultDirectoryBuilder.AssertAndConstructDirectory(dir);
    var selectedHeuristics = heuristicAsserter.AssertHeuristicsExists(heuristics);
    var collapsed = directoryAssert.Collapse(selectedHeuristics);
    if (collapsed.IsFailure)
    {
        var errors = ErrorStringFormatter.Format(collapsed.Errors);
        Console.WriteLine(errors);
        return Task.CompletedTask;
    }
    
    if (directoryAssert.Collapse(selectedHeuristics).IsFailure)
        return Task.CompletedTask;
        
    var (outDir, catSystemFile, seqsDir) = directoryAssert.Value;
    SequenceFileSynthesisRunner runner = new SequenceFileSynthesisRunner(problemLoader, problemMapper, synthesizerCollection, experimentRunner);

    var sequencesFiles = seqsDir
        .EnumerateFiles()
        .Where(e => e.Name.EndsWith(".json"));
        
    var writeResults = await runner
        .RunWithHeuristics(sequencesFiles,catSystemFile, outDir, algorithm.ToLower(), selectedHeuristics.Value, timeOut);
        
    var benchmarks = writeResults.Aggregate();
    if (benchmarks.IsFailure)
    {
        Console.WriteLine(ErrorStringFormatter.Format(benchmarks.Errors));
        return Task.CompletedTask;
    }

    Console.WriteLine("Experiment(s) completed!");
    return Task.CompletedTask;
});

consoleApp.AddCommand("synth-dir-unguided", async (
    [Argument(Description =
        "The directory containing a .json file describing system and a subdirectory /seqs/ containing sequences the topology of the system.")]
    string dir,
    [Argument]
    string algorithm,
    [Option(Description = $"The timeout in minutes. Default is 60 minutes.")]
    int? timeOut,
    JsonSynthesisProblemLoader problemLoader,
    SynthesisProblemMapper problemMapper,
    ExperimentRunner experimentRunner,
    SynthesizerCollection synthesizerCollection
) =>
{
        
    var directoryAssert = ResultDirectoryBuilder.AssertAndConstructDirectory(dir);
    
    var (outDir, catSystemFile, seqsDir) = directoryAssert.Value;
    SequenceFileSynthesisRunner runner = new SequenceFileSynthesisRunner(problemLoader, problemMapper, synthesizerCollection, experimentRunner);

    var sequencesFiles = seqsDir
        .EnumerateFiles()
        .Where(e => e.Name.EndsWith(".json"));
        
    var writeResults = await runner.RunUnguided(sequencesFiles,catSystemFile, outDir, algorithm.ToLower(), timeOut);
        
    var benchmarks = writeResults.Aggregate();
    if (benchmarks.IsFailure)
    {
        Console.WriteLine(ErrorStringFormatter.Format(benchmarks.Errors));
        return Task.CompletedTask;
    }

    Console.WriteLine("Experiment(s) completed!");
    return Task.CompletedTask;
});


consoleApp.AddCommand("heuristics",
    (HeuristicCollection collection) => Console.WriteLine("Possible heuristics are:\n " +collection.HeuristicNameDescriptions.CharSeparate('\n')));

consoleApp.AddCommand("algorithms", (SynthesizerCollection collection) =>
    {

        Console.WriteLine("Possible guided synthesis strategies which must be combined with heuristics are\n" + collection.AllGuidedKeys.CharSeparate('\n'));
        Console.WriteLine();
        Console.WriteLine("Possible unguided synthesis strategies are\n " + collection.AllUnguidedKeys.CharSeparate('\n'));
    });



consoleApp.AddCommand("synth", async (
    [Argument(Description = "The system file to run")]
    string system,
    [Argument(Description = "The problem instance")]
    string instance,
    [Argument]
    string algorithmName,
    [Argument(Description = $"The timeout in minutes")]
    int timeOut,
    [Option(Description = "Which heuristics to use. Use command 'heuristics' to list possible heuristics. Must be provided for guided search algorithms")]
    string? heuristic,
    HeuristicCollection heuristicCollection,
    JsonSynthesisProblemLoader problemLoader,
    SynthesisProblemMapper problemMapper,
    ScheduleCatVerifier verifier,
    SynthesizerCollection synthesizerCollection
) =>
{
    algorithmName = algorithmName.ToLower();
    
    if (timeOut < 1) throw new Exception("Invalid timeout.");
    var isGuidedSearch = InputAssertion
        .ExamineIfGuidedOrUnguidedSearch(algorithmName, heuristic, heuristicCollection, synthesizerCollection);

     var problemTuple = ProblemFilesFinder.FindProblemTuple(system, instance);
     if (problemTuple.IsFailure)
         throw new Exception(ErrorStringFormatter.Format(problemTuple.Errors).ToString());

     var (systemFile, inputSequenceFile) = problemTuple.Value;
    
    
    var schedulingProblemConstruction = problemLoader
        .ParseSingle( systemFile, inputSequenceFile)
        .MapTo(problemMapper.ConstructSchedulingProblem);


    if (schedulingProblemConstruction.IsFailure)
        throw new Exception("A problem occured parsing the system:" + ErrorStringFormatter.Format(schedulingProblemConstruction.Errors));
    
    var schedulingProblem = schedulingProblemConstruction.Value;
    
    ICatSynthesiser synthesiser;
    string problemKey;
    if (isGuidedSearch)
    {
        ISearchHeuristic searchHeuristic = heuristicCollection.GetResult(heuristic!).Value;
        synthesiser = synthesizerCollection.GetGuidedByKey(algorithmName, searchHeuristic, schedulingProblem).Value;
        problemKey = synthesiser.Name + "-" + searchHeuristic.PaperName;
    }
    else
    {
        synthesiser = synthesizerCollection.GetUnguidedSynthesizer(schedulingProblem, algorithmName).Value;
        problemKey = synthesiser.Name;
    }
    
    
    SingletonRunner runner = new SingletonRunner(verifier, TimeSpan.FromMinutes(timeOut));
    var (scheduleResult, verificationErrors, timeSpent, numberConfigurations ) = await runner.Run(synthesiser, schedulingProblem);

    if (!string.IsNullOrWhiteSpace(verificationErrors))
    {
        Console.Error.Write(verificationErrors);
        Console.Error.WriteLine("\n\n" + scheduleResult.Value.ToTimelineString() + "\n\n" + scheduleResult.Value);

        var t = new CsvRow(problemKey, system, instance, timeSpent.TotalSeconds,numberConfigurations, scheduleResult.Value.TotalMakespan, false, "VerificationError");
        Console.WriteLine(t.ToCsvRow());
        return;
    }

    if (scheduleResult.IsFailure)
    {
        var crow = new CsvRow(problemKey, system, instance, timeSpent.TotalSeconds, numberConfigurations, -1, false, "NoScheduleFound");
        Console.WriteLine(crow.ToCsvRow());
        return;
    }


    CsvRow row = new CsvRow(problemKey, system, instance, timeSpent.TotalSeconds,numberConfigurations, scheduleResult.Value.TotalMakespan, true, "None");
    Console.WriteLine(row.ToCsvRow());
});

consoleApp.Run();