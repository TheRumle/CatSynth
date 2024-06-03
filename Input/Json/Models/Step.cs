using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

public record Step
{
    [JsonProperty("machine")]
    public string Machine { get; set; }

    [JsonProperty("minTime")]
    public int MinTime { get; set; }

    [JsonProperty("maxTime")]
    public int MaxTime { get; set; }
}