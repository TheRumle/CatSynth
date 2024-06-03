using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

/// <summary>
/// Represents a json structure of all static information for a CAT-system.
/// Everything except the InputSequence 
/// </summary>
public record CatSystemModel
{
    [JsonProperty("Name")]
    public string SystemName { get; set; }    
    
    
    [JsonProperty("crit")]
    public List<DeadlineIndex> Deadline { get; set; }   

    [JsonProperty("protocol")]
    public Dictionary<string, List<Step>> Protocol { get; set; }

    [JsonProperty("machines")]
    public List<MachineModel> Machines { get; set; }

    [JsonProperty("arms")]
    public List<ArmModel> Arms { get; set; }
    [JsonProperty("exits")]
    public List<string> Exits { get; set; }
}