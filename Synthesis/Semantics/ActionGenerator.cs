using System.Collections.Frozen;
using Combinatorics.Collections;
using ModelChecker.Domain;
using ModelChecker.Domain.Actions;

namespace ModelChecker.Semantics;

internal static class ActionGenerator
{
    public static IEnumerable<SystemAction> GetPossibleActions(Configuration configuration)
    {
        IEnumerable<SystemAction> machineStarts = CreateMachineStarts(configuration);
        IEnumerable<SystemAction> machineStops = CreateMachineStops(configuration);
        IEnumerable<SystemAction> exits = CreateExits(configuration);
        IEnumerable<SystemAction> pickups = CreatePartPickUps(configuration);
        IEnumerable<SystemAction> placements = CreatePartPlacements(configuration);
        return machineStarts
            .Concat(machineStops)
            .Concat(exits)
            .Concat(pickups)
            .Concat(placements);
    }

    public static IEnumerable<Placement> CreatePartPlacements(Configuration configuration)
    {
        foreach (var (arm, parts) in configuration.ConfigurationsOfArms)
        {
            if(parts.Length == 0)
                 continue;
            
            //All times must have same time, so we can just check first
            if (parts.First().TimeProcessed != arm.Time)
                continue;
            
            foreach (var placement in CreatePlacements(configuration, arm, parts)) 
                yield return placement;
        }
    }

    private static IEnumerable<Placement> CreatePlacements(Configuration configuration, Arm arm, Product[] parts)
    {
        foreach (Machine machine in arm.ReachableMachines)
        {
            if (configuration.GetStateOf(machine) == MachineState.On)
                continue;
            if (parts.Length + configuration.SizeOf(machine) > machine.Capacity)
                continue;

            //If the machine we place in is active, all products must have that machine as next step
            if (machine.State == MachineState.Active && 
                parts.Any(prod => !machine.Equals(prod.Head.Machine)))
                continue;

            yield return new Placement(machine, arm, parts);
        }
    }

    public static IEnumerable<Pickup> CreatePartPickUps(Configuration configuration)
    {
        foreach (var (arm, armParts) in configuration.ConfigurationsOfArms)
        {
            if (armParts.Length != 0) continue;
            foreach (Machine machine in arm.ReachableMachines)
            {
                foreach (var pickup in CreatePickupCombinations(configuration, machine, arm)) 
                    yield return pickup;
            }
        }
    }

    private static IEnumerable<Pickup> CreatePickupCombinations(Configuration configuration, Machine machine, Arm arm)
    {
        var machineState = configuration.GetStateOf(machine);
        if (machineState is MachineState.On) yield break;
        var products = configuration.GetConfigurationOf(machine);
        if (products.Length == 0)
            yield break;

        if (machineState is MachineState.Active)
        {
            //Only products that are processed for long enough can be picked up
            var doneProducts = products.Where(e => e.TimeProcessed >= e.Head.MinProcessingTime);
            foreach (var pickup in CreatePickupCombinations(doneProducts, arm, machine))
                yield return pickup;
        }
        else if (machineState is MachineState.Off)
        {
            //all parts can be picked up in off machines
            foreach (var pickup in CreatePickupCombinations(products, arm, machine))
                yield return pickup;
        }
    }

    private static IEnumerable<Pickup> CreatePickupCombinations(IEnumerable<Product> doneProducts, Arm arm, Machine machine)
    {
        var frozenProds = doneProducts.ToFrozenSet();
        var possibleCombinations = 
            Enumerable.Range(1, Math.Min(frozenProds.Count, arm.Capacity))
            .Select(size => CreateCombinations(frozenProds, size));

        foreach (var combination in possibleCombinations)
        foreach (var combo in combination)
            yield return new Pickup(machine, arm, combo);
    }

    private static Combinations<Product> CreateCombinations(FrozenSet<Product> parts, int size)
    {
        return new Combinations<Product>(parts, size, GenerateOption.WithRepetition);
    }

    public static IEnumerable<ExitPlacement> CreateExits(Configuration configuration)
    {
        
        foreach (var conf in configuration.ConfigurationsOfArms)
        {
            //All products in arms have the same time, so we can just check first product's time
            //to see if we can place it
            if (conf.parts.Length == 0 
                || conf.arm.ReachableExits.Length == 0 
                || conf.parts.First().TimeProcessed != conf.arm.Time )
                continue;
            
            if (!conf.parts.All(part => part.IsCompleted))
                continue;
            
            foreach (var exit in conf.arm.ReachableExits)
            {
                yield return new ExitPlacement(conf.arm, exit);
            }
        }
    }

    public static IEnumerable<MachineStop> CreateMachineStops(Configuration configuration)
    {
        foreach (var config in configuration.ConfigurationsOfMachines)
        {
            if (config.machine.State is not MachineState.On)
                continue;
            
            //All parts have same processing time in the machine when it is on. We cannot have one product be started later than some other  
            if (config.parts.Any(e => e.TimeProcessed < config.parts.First().Head.MinProcessingTime))
                continue;

            yield return new MachineStop(config.machine);
        }
    }

    public static IEnumerable<MachineStart> CreateMachineStarts(Configuration configuration)
    {
        foreach (var conf in configuration.ConfigurationsOfMachines)
        {
            if (conf.machine.State is not MachineState.Off)
                continue;
            
            if (!conf.machine.RequiredAmounts.Contains(conf.parts.Length))
                continue;

            if (conf.parts.Any(e => !conf.machine.Equals(e.Head.Machine))) 
                continue;
            yield return new MachineStart(conf.machine);
        }
    }
}