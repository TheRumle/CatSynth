using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

public record CatProblemModel : CatSystemModel
{
    [JsonProperty("input")]
    public InputSequenceModel InputSequenceModel { get; set; }
    
}