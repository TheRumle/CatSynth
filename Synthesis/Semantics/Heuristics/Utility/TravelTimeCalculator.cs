using Common.Math.Discrete.Graph;
using ModelChecker.Domain;
using ModelChecker.Problem;

namespace ModelChecker.Semantics.Heuristics.Utility;

public class TravelTimeCalculator
{
    private readonly ShortestDistance<Location> _distanceLookup;
    private readonly InputSequence _inputSequence;
    private readonly ProtocolList _protocol;

    public TravelTimeCalculator(Configuration configuration, InputSequence inputSequence, ProtocolList list)
    {
        _distanceLookup = CreateLookup(configuration);
        _inputSequence = inputSequence;
        _protocol = list;
    }
    
    private ShortestDistance<Location> CreateLookup(Configuration configuration)
    {
        Dictionary<(Location, Location), float> vertices = configuration.AllLocations.OfType<Arm>()
            .SelectMany(arm => CreateVerticesBetween(arm, arm.ReachableMachines.ToList()))
            .ToDictionary();

        return FloydWarshallBuilder.CreateGraphLookup(vertices);
    }

    private float SumMoveTime(Location arm, Product part)
    {
        return part.RemainingProtocol.Sum(step => _distanceLookup.LowestCost(arm, step.Machine));
    } 
    
    private float SumRemainingPartTimeExceptHead(Location arm, Product part)
    {
        return part.RemainingProtocol.Skip(1).Sum(step => _distanceLookup.LowestCost(arm, step.Machine));
    }

    public float TimeToTraverseProtocol(IEnumerable<ProtocolStep> steps)
    {
        var protocolSteps = steps as ProtocolStep[] ?? steps.ToArray();
        var pairs = protocolSteps.Zip(protocolSteps.Skip(1), (a, b) => (first: a.Machine,second: b.Machine));
        return pairs.Select(pair => _distanceLookup.LowestCost(pair.first, pair.second)).Sum();
    }

    public float TravelTimeForForInputAndSystem(Configuration configuration)
    {
        var timeForPartsInArm = configuration
            .ConfigurationsOfArms
            .Sum(conf => conf.parts.Sum(part=> -part.TimeProcessed + SumMoveTime( conf.arm, part)));
        
        var timeToReachNextSteps =  configuration.PartsByLocation
            .Sum(conf =>
                conf.Value.Sum(part => SumRemainingPartTimeExceptHead(conf.Key, part)));

        var inputSequenceTravelTime = _inputSequence.InputSequenceAtTime(configuration.Time)
            .Select(product => _protocol.ProtocolFor(product))
            .Select(e => TimeToTraverseProtocol(e.AllSteps))
            .Sum();
        

        return timeForPartsInArm + timeToReachNextSteps + inputSequenceTravelTime;
    }

    public IEnumerable<float> MinTravelTimeForNotInputted(Configuration configuration)
    {
        var a = _inputSequence.InputSequenceAtTime(configuration.Time)
            .Select(product => _protocol.ProtocolFor(product))
            .Select(e => TimeToTraverseProtocol(e.AllSteps));
        return a;
    }



    private Dictionary<(Location, Location), float> CreateVerticesBetween(Arm arm,
        List<Machine> reachableMachine)
    {
        var graph = new Dictionary<(Location, Location), float>();
        foreach (var machine in reachableMachine)
        {
            graph[(arm, machine)] = (float)arm.Time / arm.Capacity;
            graph[(machine, arm)] = 0;
        }

        return graph;
    }
}