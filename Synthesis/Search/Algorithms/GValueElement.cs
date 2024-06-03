namespace ModelChecker.Search.Algorithms;

public class GValueElement
{
    public GValueElement(ReachableConfiguration configuration)
    {
        Configuration = configuration;
        GValue = configuration.ReachedConfiguration.Time;
    }
    
    
    
    public readonly ReachableConfiguration Configuration;
    public float GValue { get; set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        var other = ((GValueElement)obj);
        return Configuration.ReachedConfiguration.Equals(other.Configuration.ReachedConfiguration);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Configuration.ReachedConfiguration.GetHashCode();
    }
}