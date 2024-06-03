using Common;
using Common.Json;
using Common.Json.Validation;
using Common.Results;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json;

public class JsonSynthesisProblemLoader(IValidator<CatProblemModel> validator, IJsonParser<CatSystemModel> systemParser, IJsonParser<InputSequenceModel> inputSequenceParser)
{
    
    public Result<IEnumerable<CatProblemModel>> ParseToTarget(FileInfo catSystemFile, IEnumerable<FileInfo> inputSequences)
    {
        var models = inputSequences
            .Select(ReadFile)
            .Select(e => AssignInputSequence(e, File.ReadAllText(catSystemFile.FullName)));
        
        var allErrors = GetErrors(models);
        return allErrors.Any() 
            ? Result.Failure<IEnumerable<CatProblemModel>>(allErrors.ToArray()) 
            : Result.Success(models.Select(e => e.Value));
    }
    
    public Result<CatProblemModel> ParseSingle(FileInfo catSystemFile, FileInfo inputSequences)
    {
        var models = ReadFile(inputSequences);
        return AssignInputSequence(models,File.ReadAllText(catSystemFile.FullName));
    }
    
    public Result<CatProblemModel> ParseCatProblemFiles(FileInfo catSystemFile, FileInfo inputSequences)
    {
        var models = ReadFile(inputSequences);
        var catProblem = AssignInputSequence(models,File.ReadAllText(catSystemFile.FullName));
        return catProblem;
    }

    private IEnumerable<Error> GetErrors(IEnumerable<Result<CatProblemModel>> models)
    {
        var parseErrors = models.Where(e => e.IsFailure).SelectMany(e => e.Errors);
        
        var validationErrors = models.Where(e=>e.IsSuccess).SelectMany(e => validator.Validate(e.Value).ToErrors());
        return validationErrors.Union(parseErrors);
    }

    private Result<CatProblemModel> AssignInputSequence(Result<(InputSequenceModel model, string fileName)> inputSequenceModel, string jsonString)
    {
        if (inputSequenceModel.IsFailure)
            return Result.Failure<CatProblemModel>(inputSequenceModel.Errors);
        
        return systemParser
            .ParseToTarget(jsonString)
            .MapTo(e=> new CatProblemModel
        {
            Arms = e.Arms,
            InputSequenceModel = inputSequenceModel.Value.model,
            Deadline = e.Deadline,
            Exits = e.Exits,
            Machines = e.Machines,
            SystemName = e.SystemName,
            Protocol = e.Protocol
        });
    }


    private Result<(InputSequenceModel inputSeq, string fileName)> ReadFile(FileInfo fileInfo)
    {
        var text = File.ReadAllText(fileInfo.FullName);
        return inputSequenceParser.ParseToTarget(text).Combine(fileInfo.Name);
    }
}