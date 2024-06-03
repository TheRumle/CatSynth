using Common;
using Common.Json;
using Common.Results;
using Newtonsoft.Json;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis;

public class InputSequenceParser : IJsonParser<InputSequenceModel>
{
    /// <inheritdoc />
    public Result<InputSequenceModel> ParseToTarget(string jsonString)
    {
        try
        {
            var model = JsonConvert.DeserializeObject<InputSequenceModel>(jsonString, Options.Settings);
            return Result.Success(model)!;
        }
        catch (JsonException e)
        {
            return Result.Failure<InputSequenceModel>(new Error("JsonException", e.ToString()));
        }
        catch (Exception e)
        {
            return Result.Failure<InputSequenceModel>(new Error("Unexpected", $"Got unexpected error when parsing {nameof(InputSequenceModel)}: {e}"));
        }
    }
}