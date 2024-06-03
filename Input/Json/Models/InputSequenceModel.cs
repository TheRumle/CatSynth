using Newtonsoft.Json;

namespace Scheduler.Input.Json.Models;

public record InputSequenceModel
{
    [JsonProperty("sequence")]
    public List<string> Sequence { get; set; }

    [JsonProperty("every")]
    public int Every { get; set; }
    
    
    [JsonProperty("batchSize")]
    public int BatchSize { get; set; }
    
    [JsonProperty("inputMachine")]
    public string InputMachine { get; set; }
}