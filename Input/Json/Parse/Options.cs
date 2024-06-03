using Newtonsoft.Json;

namespace Scheduler.Input.Json.Parse;

public static class Options
{
    public static JsonSerializerSettings Settings = new()
    {
        NullValueHandling =  NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };
}