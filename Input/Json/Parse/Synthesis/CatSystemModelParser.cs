using Common.Json;
using Common.Json.Validation;
using Common.Results;
using Newtonsoft.Json;
using Scheduler.Input.Json.Models;
using Scheduler.Input.Json.Parse.Synthesis.Validators;

namespace Scheduler.Input.Json.Parse.Synthesis;

public class CatSystemModelParser : IJsonParser<CatSystemModel>
{
    
    /// <inheritdoc />
    public Result<CatSystemModel> ParseToTarget(string jsonString)
    {
        var systemModel = JsonConvert.DeserializeObject<CatSystemModel>(jsonString, Options.Settings);
        CatSystemValidator validator = new CatSystemValidator();
        var errors = validator.Validate(systemModel!).ToErrors();
        return errors.Any() 
            ? Result.Failure<CatSystemModel>(errors.ToArray()) 
            : Result.Success(systemModel!);
    }
}