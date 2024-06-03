using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

public record DeadlineIndex
{
    [JsonProperty("product")]
    public string Product { get; set; }
    [JsonProperty("start")]
    public int Start { get; set; }  
    [JsonProperty("end")]
    public int End { get; set; }
    [JsonProperty("time")]
    public int Time { get; set; }
}