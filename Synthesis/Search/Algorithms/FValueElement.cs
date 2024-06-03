using ModelChecker.Domain;

namespace ModelChecker.Search.Algorithms;

public class FValueElement : IComparable<FValueElement>
{
    public FValueElement(ReachableConfiguration configuration, float fValue)
    {
        ReachedConfiguration = configuration;
        GValue = configuration.ReachedConfiguration.Time;
        FValue = fValue;
        Configuration = configuration.ReachedConfiguration;
    }


    public float FValue { get; set; }
    public readonly ReachableConfiguration ReachedConfiguration;
    public readonly Configuration Configuration;
    public float GValue { get; set; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        var other = ((FValueElement)obj);
        return Configuration.Equals(other.Configuration);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return ReachedConfiguration.ReachedConfiguration.GetHashCode();
    }

    /// <inheritdoc />
    public int CompareTo(FValueElement? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return FValue.CompareTo(other.FValue);
    }
}