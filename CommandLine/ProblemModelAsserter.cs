using Common;
using Common.Results;
using ModelChecker.Problem;
using Scheduler.Input.Json;
using Scheduler.Input.Json.Parse.Synthesis;

public class ProblemModelAsserter(JsonSynthesisProblemLoader problemLoader,SynthesisProblemMapper synthesisProblemMapper)
{
    public Result<SchedulingProblem> AssertProblemModel(FileInfo catSystemFile, FileInfo sequenceFile)
    {
        var problemModel = problemLoader.ParseCatProblemFiles(catSystemFile, sequenceFile);
        if (problemModel.IsFailure)
        {
            Console.WriteLine(ErrorStringFormatter.Format(problemModel.Errors));
            return Result.Failure<SchedulingProblem>();
        }
        return Result.Success(synthesisProblemMapper.ConstructSchedulingProblem(problemModel.Value));
    }
}