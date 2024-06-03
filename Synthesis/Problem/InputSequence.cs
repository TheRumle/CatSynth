using ModelChecker.Domain;

namespace ModelChecker.Problem;

public sealed class InputSequence
{
    private readonly int _interval;
    private readonly ProtocolList _protocols;
    private readonly string[] _sequence;
    public readonly int BatchSize;
    public readonly int DoneTime;
    public readonly Machine InputMachine;



    public InputSequence(IEnumerable<string> sequence, int interval, int batchSize, Machine inputMachine,
        ProtocolList protocols)
    {
        var seq = sequence as string[] ?? sequence.ToArray();
        if (seq.Length % batchSize != 0)
            throw new ArgumentException(
                $"The given sequence contains {seq.Length} elements which is not divisible by {batchSize}");

        if (interval <=0 )throw new ArgumentException($"The given sequence input interval {interval} must be > 0");
        
        _sequence = seq;
        _interval = interval;
        BatchSize = batchSize;
        _protocols = protocols;
        var inputsNeeded = Math.Ceiling((double)seq.Length / batchSize);
        DoneTime = (int)(inputsNeeded * interval);
        InputMachine = inputMachine;
    }


    public int TimeUntilNextInput(int time)
    {
        if (time >= DoneTime) return int.MaxValue;
        return Math.Abs(_interval - time % _interval);
    }

    public IEnumerable<string> InputSequenceAtTime(int time)
    {
        var elementsAlreadyInputted = (int)Math.Floor((double)time / _interval) * BatchSize;
        return _sequence.Skip(elementsAlreadyInputted);
    }
    
    private (int alreadyInputted, IEnumerable<string> atTime) InputSequenceAtTimeAndAlreadyInputted(int time)
    {
        var elementsAlreadyInputted = (int)Math.Floor((double)time / _interval) * BatchSize;
        return (elementsAlreadyInputted,_sequence.Skip(elementsAlreadyInputted));
    }

    /// <summary>
    ///  Gives the next batch for the time. If next batch is at time 10 is PPP, and the input time is 1-10 inclusive, then returns PPP.  
    /// </summary>
    /// <param name="time">The current configuration time</param>
    /// <returns></returns>
    public IEnumerable<Product> NextBatchAtTime(int time)
    {
        var (amountsAlready, elements) = InputSequenceAtTimeAndAlreadyInputted(time);
        return elements.Take(BatchSize).Select((type,index) => new Product(_protocols.ProtocolFor(type), 0, 0,type, 0, amountsAlready+index));
    }

    public bool IsInputTime(int time)
    {
        return time > 0 && 
               (TimeUntilNextInput(time) == this._interval || time == DoneTime);
    }
}