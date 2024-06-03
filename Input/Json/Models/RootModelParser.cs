using Common.Json;
using Common.Json.Validation;
using Common.Results;
using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

public class RootModelParser(IValidator<CatProblemModel> validator) : IJsonParser<CatProblemModel>
{
    public Result<CatProblemModel> ParseToTarget(string jsonString)
    {
        var jsonModel = JsonConvert.DeserializeObject<CatProblemModel>(jsonString, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        });
        if (jsonModel is null) throw new ArgumentException($"The json string is malformed and could not be parsed to a root model");
        

        var errs = validator.Validate(jsonModel).ToArray();
        return errs.Length != 0
            ? errs.ToFailedResult<CatProblemModel>() 
            : Result.Success(jsonModel);
    }
    
}