using ModelChecker.Problem;

namespace ModelChecker.Domain;

public record struct Product
{
    private readonly Protocol _protocol;
    public readonly int CompletedSteps;
    public readonly int Id;
    public readonly bool IsCompleted;
    public readonly string PartType;
    public readonly IEnumerable<ProtocolStep> RemainingProtocol;
    public readonly int RemainingSteps;
    public readonly int TimeProcessed;
    public readonly int UnstableTime;
    public readonly ProtocolStep Head;

    internal Product(Protocol protocol, int timeProcessed, int unstableTime, string partType, int stepsCompleted, int id)
    {
        _protocol = protocol;
        UnstableTime = unstableTime;
        TimeProcessed = timeProcessed;
        PartType = partType;
        RemainingProtocol = protocol.SkipSteps(stepsCompleted);
        CompletedSteps = stepsCompleted;
        Id = id;
        RemainingSteps = protocol.TotalLength - stepsCompleted;
        IsCompleted = protocol.TotalLength <= CompletedSteps;
        ProtocolStep head = RemainingProtocol.FirstOrDefault();
        Head = head.Equals(default) 
            ? new ProtocolStep(null!, int.MaxValue, int.MaxValue) 
            : head;
    }

    /// <inheritdoc />
    public bool Equals(Product other)
    {
        return UnstableTime == other.UnstableTime && PartType == other.PartType &&
               TimeProcessed == other.TimeProcessed && CompletedSteps == other.CompletedSteps;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(UnstableTime, PartType, TimeProcessed, CompletedSteps);
    }



    public Product ToNonProcessed()
    {
        return new(_protocol, 0, UnstableTime, PartType, CompletedSteps, Id);
    }

    public Product WithTimeDeadlineProgression(int amount)
    {
        return new Product(this._protocol, TimeProcessed, UnstableTime + amount, PartType, CompletedSteps, Id );
    }

    public Product WithProcessingTimeProgression(int amount)
    {
        return new Product(this._protocol, TimeProcessed+amount, UnstableTime, PartType, CompletedSteps, Id);
    }
    
    public Product WithProcessingAndDeadlineTimeProgression(int amount)
    {
        return new Product(_protocol, TimeProcessed+amount, UnstableTime + amount, PartType, CompletedSteps, Id);
    }

    
    public Product Copy()
    {
        return new Product(_protocol, TimeProcessed, UnstableTime, PartType, CompletedSteps, Id);
    }

    public Product CreateProgressedCopy()
    {
        return new Product(_protocol,  0, UnstableTime, PartType, CompletedSteps + 1, Id);
    }
    
    /// <inheritdoc />
    public override string ToString()
    {
        return
            $"ID:{Id}, {nameof(UnstableTime)}: {UnstableTime},  ProcessingTime: {TimeProcessed}, {nameof(PartType)}: {PartType}, {nameof(CompletedSteps)}: {CompletedSteps}, {nameof(IsCompleted)}: {IsCompleted}";
    }
}