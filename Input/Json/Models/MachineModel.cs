using ModelChecker.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scheduler.Input.Json.Models;

public record MachineModel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("state")]
    [JsonConverter(typeof(StringEnumConverter))]
    public MachineState State { get; set; }
        
    [JsonProperty("capacity")]
    public int Capacity { get; set; }
        
    [JsonProperty("req")]
    public IEnumerable<int> RequiredParts { get; set; }
}