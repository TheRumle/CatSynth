using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

public record ArmModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("reach")]
    public List<string> Reach { get; set; }
    
    [JsonProperty("time")]
    public int Time { get; set; }

    [JsonProperty("capacity")]
    public int Capacity { get; set; }
}