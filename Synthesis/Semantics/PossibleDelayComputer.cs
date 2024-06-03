using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Exceptions;
using ModelChecker.Problem;

namespace ModelChecker.Semantics;

internal sealed class PossibleDelayComputer
{
    private readonly DeadlineCollection _deadlineCollection;
    private readonly InputSequence _inputSequence;

    public PossibleDelayComputer(DeadlineCollection deadlineCollection, InputSequence inputSequence)
    {
        _deadlineCollection = deadlineCollection;
        _inputSequence = inputSequence;
    }


    public Delay[] ComputePossibleDelays(Configuration configuration)
    {

        var delayTuple = ComputePossibleDelaySpan(configuration);
        
        
        return delayTuple switch
        {
            (_, < 0) => throw new InvalidConfigurationException("Computed a max delay < 0"),
            (< 0, _) => throw new InvalidConfigurationException("Computed a min delay < 0"),
            (_, >= int.MaxValue) => [],
            (_, 0) => [],
            (_, 1) => [CreateSingle(1)],
            (0, var maxDelay) => CreateRange(1, maxDelay),
            var (minDelay, maxDelay) => CreateRange(minDelay, maxDelay)
        };
    }

    private int TimeUntilDeadline(Configuration configuration)
    {
        return _deadlineCollection
            .TimeUntilDeadlines(configuration.AllParts)
            .DefaultIfEmpty(int.MaxValue).Min();
    }

    private Delay[] CreateRange(int minDelay, int maxDelay)
    {
        if (minDelay == maxDelay)
            return[new Delay(minDelay)];

        return Enumerable
            .Range(minDelay, maxDelay - minDelay + 1)
            .Select(delay => new Delay(delay))
            .ToArray();
    }

    private Delay CreateSingle(int delayOf)
    {
        return new Delay(delayOf);
    }


    private (int MinDelay, int MaxDelay) ComputePossibleDelaySpan(Configuration configuration)
    {
        
        var deadlineLimit = TimeUntilDeadline(configuration);
        var (minArmTimes, maxArmTimes) = ComputeArmLimits(configuration);
        var (minProcessingTime, maxProcessingTime) = ComputeProcessingLimits(configuration);
        var timeUntilInput = ComputeInputMax(configuration, [deadlineLimit, maxArmTimes, maxProcessingTime]);
        (int[] mins, int[] maxs) minMaxes =
        (
            [timeUntilInput, minArmTimes, minProcessingTime], //each of the event are relevant
            [timeUntilInput, deadlineLimit, maxArmTimes, maxProcessingTime ] //each of the events limit the passing of time
        );
        
        (int minDelay, int maxDelay) result = (minMaxes.mins.Min(), minMaxes.maxs.Min());
        result.minDelay = result.minDelay == int.MaxValue ? 0 : result.minDelay;
        result.maxDelay = result.maxDelay == int.MaxValue ? result.minDelay : result.maxDelay;

        if (result.minDelay > result.maxDelay)
        {
            result.minDelay = result.maxDelay;
        }

        if (result.maxDelay < 0 || result.maxDelay == int.MaxValue) 
            return (0, 0);
        
        return result;
    }

    private int ComputeInputMax(Configuration configuration, IEnumerable<int> limits)
    {   
        var timeUntilNextInput = _inputSequence.TimeUntilNextInput(configuration.Time);
        if (_inputSequence.InputMachine.Capacity < configuration.SizeOf(_inputSequence.InputMachine) + _inputSequence.BatchSize)
            return Math.Min(timeUntilNextInput-1,limits.Min());

        return timeUntilNextInput;
    }


    private (int min, int max) ComputeArmLimits(Configuration configuration)
    {
        var maxDelay = int.MaxValue;
        var minDelay = int.MaxValue;
        
        var armParts = configuration.ConfigurationsOfArms.Where(e=>e.parts.Length > 0);
        
        
        foreach (var (arm, parts) in armParts)
        {
            Product product = parts.MaxBy(e => e.TimeProcessed)!;

            var timeUntilPartDone = arm.Time - product.TimeProcessed;
            if (timeUntilPartDone == 0) return (0, 0);

            maxDelay = Math.Min(maxDelay, timeUntilPartDone);

            minDelay = Math.Min(
                minDelay,
                timeUntilPartDone
            );
        }
        return (minDelay, maxDelay);
    }

    private (int minDelay, int maxDelay) ComputeProcessingLimits(Configuration configuration)
    {
        var mParts = configuration
            .ConfigurationsOfProcessingMachines
            .Where(e => e.parts.Length > 0);

        var maxDelay = int.MaxValue;
        var minDelay = int.MaxValue;
        
        foreach (var (machine, parts) in mParts)
        {
            Product product = parts.MaxBy(e => e.TimeProcessed)!;
            var head = product.Head;
            var maxProcessingTime = head.MaxProcessingTime;
            if (maxProcessingTime == product.TimeProcessed) return (0, 0);

            maxDelay = Math.Min(maxDelay, maxProcessingTime - product.TimeProcessed);

            minDelay = Math.Min(
                minDelay,
                Math.Max(head.MinProcessingTime - product.TimeProcessed, 0)
            );
        }
        return (minDelay, maxDelay);
    }
}