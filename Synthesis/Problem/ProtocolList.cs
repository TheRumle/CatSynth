namespace ModelChecker.Problem;

public sealed class ProtocolList
{
    private readonly Dictionary<string, Protocol> _protocols;

    public ProtocolList(Dictionary<string, Protocol> protocols)
    {
        _protocols = protocols;
    }

    public Protocol ProtocolFor(string part)
    {
        return _protocols[part];
    }
}