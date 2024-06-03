using Common.Results;

namespace Common.Json;

public interface IJsonParser<TTarget>
{
    public Result<TTarget> ParseToTarget(string jsonString);
}