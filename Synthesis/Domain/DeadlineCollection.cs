namespace ModelChecker.Domain;

public sealed class DeadlineCollection
{
    private struct JourneyDeadline
    {
        public void Deconstruct(out int begin, out int end, out int maxAge)
        {
            begin = Begin;
            end = End;
            maxAge = MaxAge;
        }

        public required int Begin;
        public required int End;
        public required int MaxAge;
    }
    private readonly Dictionary<string, JourneyDeadline> _deadlines = new();

    public DeadlineCollection(IEnumerable<(string PartType, int DeadlineBegin, int DeadlineEnd, int MaxAge)> critical)
    {
        foreach (var valueTuple in critical)
            _deadlines[valueTuple.PartType] = new JourneyDeadline
            {
                Begin = valueTuple.DeadlineBegin,
                End = valueTuple.DeadlineEnd,
                MaxAge = valueTuple.MaxAge
            };
    }

    public IEnumerable<int> TimeUntilDeadlines(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            yield return TimeUntilDeadline(product);
        }
    }

    public int TimeUntilDeadline(Product product)
    {
        if (_deadlines.TryGetValue(product.PartType, out var deadline)
            && IsWithinDeadline(product, deadline))
        {
            return deadline.MaxAge - product.UnstableTime;
        }

        return int.MaxValue;
    }

    public bool IsCritical(Product product)
    {
        return _deadlines.TryGetValue(product.PartType, out var deadline)
               && IsWithinDeadline(product, deadline);
    }
    
    private static bool IsWithinDeadline(Product product, JourneyDeadline deadline)
    {
        var journeyCount = product.CompletedSteps;
        return (journeyCount >= deadline.Begin && journeyCount <= deadline.End);
    }
    
    public (int start, int end, int deadline) GetDeadline(Product p)
    {
        var (s, e, d) = _deadlines[p.PartType];
        return (s, e, d);
    }





}