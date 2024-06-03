using System.Collections;
using System.Text;

namespace Cat.Verify.Definitions;

public sealed record Configuration : IEnumerable<KeyValuePair<string, (string location, int stepsCompleted)>>
{
    private readonly Dictionary<string, (string location, int stepsCompleted)> _configuration;
    public static (string location, int stepsCompleted) Bot = (CatContext.BOT, -1);

    public Configuration(Dictionary<string, (string location, int stepsCompleted)> configuration)
    {
        _configuration = configuration;
    }
    
    /// <inheritdoc />
    public bool Equals(Configuration? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        foreach (var (productId, (location,stepsCompleted)) in _configuration)
        {
            var exists = other._configuration.TryGetValue(productId, out var otherConf);
            if (!exists) return false;
            if (location != otherConf.location || stepsCompleted != otherConf.stepsCompleted) return false;
        }

        return true;
    }


    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, (string location, int stepsCompleted)>> GetEnumerator()
    {
        return _configuration.GetEnumerator();
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _configuration.GetHashCode();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }



    public (string location, int stepsCompleted) this[string productId]
    {
        get => _configuration.ContainsKey(productId) ? _configuration[productId] : Bot;
        set => _configuration[productId] = value;
    }

    public IEnumerable<string> ContentOf(string machine)
    {
        return _configuration.Where(e => e.Value.location == machine).Select(e => e.Key);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder bob = new();
        var groups = this.GroupBy(prodConf => prodConf.Value.location)
            .OrderBy(e=>e.Key);
        
        foreach (var a in groups)
        {
            bob.Append($"{a.Key}: {string.Join(", ", a.Select(e => e.Key))}\t\t");
        }

        return bob.ToString();
    }
}