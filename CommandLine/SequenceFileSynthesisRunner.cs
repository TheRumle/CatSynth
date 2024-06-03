using Common;
using Common.Json.Validation;
using Common.Results;
using Common.String;
using Experiments.ExperimentDriver;
using Experiments.ResultWriter;
using ModelChecker;
using ModelChecker.Search;
using ModelChecker.Search.Algorithms;
using ModelChecker.Semantics.Heuristics;
using Scheduler.Input.Json;
using Scheduler.Input.Json.Parse.Synthesis;

namespace CommandLineApp;

public class SequenceFileSynthesisRunner(JsonSynthesisProblemLoader problemLoader, SynthesisProblemMapper problemMapper, SynthesizerCollection synthesizerCollection, ExperimentRunner experimentRunner)
{
    public async Task<List<Result<FileInfo>>> RunWithHeuristics(IEnumerable<FileInfo> sequencesFiles, FileInfo catSystemFile, DirectoryInfo outDir,
        string selectedAlgorithm, IEnumerable<ISearchHeuristic> heuristics, int? timeOutMinutes)
    {
        var timeOut = timeOutMinutes is null
            ? Run.DefaultTimeout
            : TimeSpan.FromMinutes(timeOutMinutes.Value);
        
        var writeResults = new List<Result<FileInfo>>();
        foreach (var sequenceFile in sequencesFiles)
        {
            ProblemModelAsserter problemModelAsserter = new(problemLoader, problemMapper);
            var schedulingProblem = problemModelAsserter.AssertProblemModel(catSystemFile, sequenceFile);
            if (schedulingProblem.IsFailure)
                throw schedulingProblem.Errors.ToException();
        
            var  writer = new ExperimentResultWriter(outDir, $"{sequenceFile.Name}.csv");
            await writer.WriteHeader();
            foreach (var heuristic in heuristics)
            {
                               
                            
                var synthesiser = synthesizerCollection.GetGuidedByKey(selectedAlgorithm.ToLower(), heuristic, schedulingProblem.Value);
                if (synthesiser.IsFailure)
                {
                    var failureString = ErrorStringFormatter.Format(synthesiser.Errors) ;
                    var options = synthesizerCollection.AllGuidedKeys.CommaSeparate();
                    throw new ArgumentException($"{failureString}. Options are: \n {options}");
                }
                
                if (heuristic is MinDelay && synthesiser.Value is CatStarDepthFirst)
                    continue;
                
                
                Console.WriteLine("Beginning " + heuristic.PaperName + " " + sequenceFile.Name);
                var result = await experimentRunner.RunHeuristicSynthesizer(synthesiser.Value, timeOut, schedulingProblem.Value, heuristic);
                
                
                
                var writeResult = await writer.WriteLine(result, heuristic.PaperName);
                writeResults.Add(writeResult);
            }
            await writer.Commit();
        }

        return writeResults;
    }

    public async Task<List<Result<FileInfo>>> RunUnguided(IEnumerable<FileInfo> sequencesFiles, FileInfo catSystemFile,
        DirectoryInfo outDir,
        string selectedAlgorithm, int? timeOutMinutes)
    {
        var timeOut = timeOutMinutes is null
            ? Run.DefaultTimeout
            : TimeSpan.FromMinutes(timeOutMinutes.Value);

        var writeResults = new List<Result<FileInfo>>();
        foreach (var sequenceFile in sequencesFiles)
        {
            ProblemModelAsserter problemModelAsserter = new(problemLoader, problemMapper);
            var schedulingProblem = problemModelAsserter.AssertProblemModel(catSystemFile, sequenceFile);
            if (schedulingProblem.IsFailure)
                throw schedulingProblem.Errors.ToException();

            var writer = new ExperimentResultWriter(outDir, $"{sequenceFile.Name}.csv");
            await writer.WriteHeader();
            
            var synthesiser = synthesizerCollection.GetUnguidedByKey(selectedAlgorithm.ToLower(), schedulingProblem.Value);
            if (synthesiser.IsFailure)
            {
                var failureString = ErrorStringFormatter.Format(synthesiser.Errors) ;
                var options = synthesizerCollection.AllUnguidedKeys.CommaSeparate();
                throw new ArgumentException($"{failureString}. Options are: \n {options}");
            }
            
            Console.WriteLine("Beginning " + sequenceFile.Name);

            var result = await experimentRunner.RunUnguidedSynthesizer(synthesiser.Value, timeOut, schedulingProblem.Value);
            var writeResult = await writer.WriteLine(result, HeuristicCollection.NoHeuristic.PaperName);
            writeResults.Add(writeResult);
            
            await writer.Commit();
        }

        return writeResults;
    }
}