using Cat.Verify;
using Cat.Verify.Definitions;
using Cat.Verify.Verification.TransitionVerification;
using ModelChecker;
using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;
using Configuration = Cat.Verify.Definitions.Configuration;
using Pickup = Cat.Verify.Definitions.Pickup;
using Placement = Cat.Verify.Definitions.Placement;
using ModelCheckerPickup = ModelChecker.Domain.Actions.Pickup;
using ModelCheckerPlacement = ModelChecker.Domain.Actions.Placement;

namespace CatConversion.SynthesisVerification;

public static class ScheduleCatConverter
{
    public static (CatContext, Execution) Convert(SchedulingProblem schedulingProblem, Schedule schedule)
    {
        return (ConstructCatContext(schedulingProblem, schedule.ReachedStates.Last().ReachedConfiguration), ConstructExecution(schedulingProblem, schedule));
    }

    private static Execution ConstructExecution(SchedulingProblem schedulingProblem, Schedule schedule)
    {
        var steps = CreateTransitions(schedule, schedulingProblem.InputSequence);
        return new Execution(steps);
    }

    private static IEnumerable<Transition> CreateTransitions(Schedule schedule, InputSequence sequence)
    {
        var timeline = TrimSchedule(schedule, sequence);
        var executionSteps = ToTransitions(sequence,timeline);
        return executionSteps;
    }

    private static IEnumerable<Transition> ToTransitions(InputSequence sequence, List<ScheduleStep> timeline)
    {
        foreach (var (from, action, reached) in timeline)
        {
            yield return action switch
            {
                ExitPlacement exitPlacement     => MapToTransition(from, reached, () => MapExit(exitPlacement, from)), 
                MachineStart machineStart       => MapToTransition(from, reached, () => new Start(machineStart.Machine.Name)),
                MachineStop machineStop         => MapToTransition(from, reached, () => new Stop(machineStop.Machine.Name)),
                ModelCheckerPickup pickup       => MapToTransition(from, reached, () => MapPickup(pickup, from)),
                ModelCheckerPlacement placement => MapToTransition(from, reached, () => MapPlacement(placement)),
                Delay d => MapToTransition(from, reached,() =>  MapInput(from.Time, sequence)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private static List<ScheduleStep> TrimSchedule(Schedule schedule, InputSequence sequence)
    {
        var timeline = schedule.ToTimeline().ToList();
        //Remove all unimportant delay actions
        timeline.RemoveAll(e=>e.action is Delay d && !sequence.IsInputTime(e.to.Time));
        for (int i = 1; i < timeline.Count; i++)
        {
            var firstStep = timeline[i-1];
            var second = timeline[i];
            firstStep.to = second.from;
        }

        return timeline;
    }


    private static Transition MapToTransition<T>
    (   ModelChecker.Domain.Configuration from,
        ModelChecker.Domain.Configuration to,

        Func<T> map) where T : ICatOperation
    {
        return new Transition(MapConfiguration(from), to.Time, map.Invoke(),MapConfiguration(to));
    }

    private static Arrival MapInput(int time, InputSequence schedulingProblemInputSequence)
    {
        var batch = schedulingProblemInputSequence.NextBatchAtTime(time).Select(e => e.Id.ToString());
        return new Arrival(
            batch,
            schedulingProblemInputSequence.InputMachine.Name);
    }


    private static Placement MapPlacement(global::ModelChecker.Domain.Actions.Placement placement)
    {
        return new Placement(placement.Parts.Select(e => e.Id.ToString()), placement.Machine.Name, placement.Arm.Name);
    }

    private static Pickup MapPickup(ModelCheckerPickup pickup, global::ModelChecker.Domain.Configuration from)
    {
        return new Pickup(pickup.Parts.Select(e=>e.Id.ToString()), pickup.Machine.Name, pickup.Arm.Name);
    }


    private static Placement MapExit(ExitPlacement exitPlacement, global::ModelChecker.Domain.Configuration prev)
    {
        var moved = prev.PartsByLocation[exitPlacement.Arm].Select(e=>e.Id.ToString());
        return new Placement(moved, CatContext.BOT, exitPlacement.Arm.Name);
    }


    private static Configuration MapConfiguration(global::ModelChecker.Domain.Configuration config)
    {
        Dictionary<string, (string Location, int stepsCompleted)> result = new();
        foreach (var productLocation in config.PartsByLocation)
        {
            var l = productLocation.Key.Name;
            if (productLocation.Key is Exit e)
            {
                l = CatContext.BOT;
            }
            
            foreach (var product in productLocation.Value)
            {
                result[product.Id.ToString()] = (l, product.CompletedSteps);
            }
        }

        return new Configuration(result);
        

    }



    private static CatContext ConstructCatContext(SchedulingProblem schedulingProblem, global::ModelChecker.Domain.Configuration configuration)
    {
        var arms = configuration.AllLocations.OfType<Arm>().Select(arm => arm.Name);
        var armTimes = configuration.AllLocations.OfType<Arm>().ToDictionary(arm => arm.Name, arm => arm.Time);
        var armReachable = configuration.AllLocations.OfType<Arm>()
            .ToDictionary(arm => arm.Name, arm => arm.ReachableExits.Select(exit => exit.Name)
                .Union(arm.ReachableMachines.Select(machine => machine.Name)));

        var capacities = configuration.AllLocations.Where(location => !(location is Exit))
            .ToDictionary(location => location.Name, location => location.Capacity);

        var prods = configuration.AllParts;
        var productIds = prods.Select(part => part.Id.ToString());
        var deadlines = prods.ToDictionary(product => product.Id.ToString(),
            id => schedulingProblem.DeadlineCollection.GetDeadline(id));

        var activeMachines = configuration.AllLocations.OfType<Machine>()
            .Where(machine => (machine.State & MachineState.Active) != 0)
            .Select(machine => machine.Name);
        var statefulMachines = configuration.AllLocations.OfType<Machine>()
            .Where(machine => (machine.State & MachineState.Off) != 0)
            .Select(machine => machine.Name);
        
        var requiredParts = configuration.AllLocations.OfType<Machine>()
            .ToDictionary(machine => machine.Name, machine => machine.RequiredAmounts.AsEnumerable());

        var protocol = configuration.AllParts.ToDictionary(
            part => part.Id.ToString(),
            part => schedulingProblem.Protocols.ProtocolFor(part.PartType).AllSteps.Select(step =>
                (step.Machine.Name, step.MinProcessingTime, step.MaxProcessingTime)));

        return new CatContext
        {
            Arms = arms,
            ProductIds = productIds,
            Exits = configuration.AllLocations.OfType<Exit>().Select(exit => exit.Name),
            ActiveMachines = activeMachines,
            Machines = statefulMachines,
            ArmTimes = armTimes,
            ArmReach = armReachable,
            Capacity = capacities,
            RequiredParts = requiredParts,
            Protocol = protocol,
            CriticalSection = deadlines
        };
    }


}

  