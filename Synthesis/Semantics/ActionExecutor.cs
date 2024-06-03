using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;
using ModelChecker.Search;

namespace ModelChecker.Semantics;

internal sealed class ActionExecutor : IActionExecutor
{
    private readonly DeadlineCollection _deadlineCollection;
    private readonly InputSequence _inputSequence;

    public ActionExecutor(DeadlineCollection deadlineCollection,
        InputSequence inputSequence)
    {
        _deadlineCollection = deadlineCollection;
        _inputSequence = inputSequence;
    }

    /// <inheritdoc />
    public Configuration Execute(SystemAction action, Configuration configuration)
    {
        var res =  action switch
        {
            Delay delay => DelayBy(delay, configuration),
            ExitPlacement exit => ExecuteExit(exit, configuration),
            MachineStart machineStart => StartMachine(machineStart, configuration),
            MachineStop machineStop => StopMachine(machineStop, configuration),
            Pickup pickup => PickUp(pickup, configuration),
            Placement placement => Place(placement, configuration),
            _ =>  throw new ArgumentOutOfRangeException($"Recieved an action that was not a known." +
                                                        $" Should not happen")
        };
        return res;
    }

    private static Configuration StopMachine(MachineStop machineStop, Configuration configuration)
    {
        var machineConf = CopyOtherConfigurations(machineStop.Machine, configuration, out var dict);
    
        var stoppedVersion = machineStop.Machine.StoppedVersion();
        dict.Add(stoppedVersion, machineConf.parts.Select(e=>e.CreateProgressedCopy()).ToArray());
        return new Configuration(dict, configuration.Time);
    }
    
    private static Configuration StartMachine(MachineStart machineStart, Configuration configuration)
    {
        var machineConf = CopyOtherConfigurations(machineStart.Machine, configuration, out var dict);

        Machine startedVersion = machineStart.Machine.StartedVersion();
        dict.Add(startedVersion, machineConf.parts);
        return new Configuration(dict, configuration.Time);
    }
    
    private static (Machine machine, Product[] parts) CopyOtherConfigurations(Machine except,
        Configuration configuration, out Dictionary<Location, Product[]> dict)
    {
        (Machine machine, Product[] parts) machineConf = configuration.ConfigurationsOfMachines.First(e => e.machine.Equals(except));

        dict = new Dictionary<Location, Product[]>();
        foreach (var location in configuration.AllLocations.Except([machineConf.machine])) 
            dict[location] = configuration.PartsByLocation[location];
        return machineConf;
    }

    private static Configuration ExecuteExit(ExitPlacement exitPlacement, Configuration c)
    {
        var toMove = c.GetConfigurationOf(exitPlacement.Arm);
        var q= AddAndRemoveFrom(c, exitPlacement.Arm, exitPlacement.E, toMove, toMove);
        return q;
    }

    private Configuration DelayBy(Delay delay, Configuration oldConfiguration)
    {
        Product[] partsToSpawn = [];
        if (delay.Amount == _inputSequence.TimeUntilNextInput(oldConfiguration.Time))
            partsToSpawn = _inputSequence.NextBatchAtTime(oldConfiguration.Time).ToArray();

        var processingLocations = GetProcessingLocations(oldConfiguration);
        var offMachineConfigurations = oldConfiguration.ConfigurationsOfMachines
            .Where(e => e.machine.State is MachineState.Off);
        
        var exitConfigurations = oldConfiguration
            .PartsByLocation
            .Where(e => e.Key is Exit);
        
        var processingDict = ProgressProcessingAndDeadlinesFor(processingLocations, delay);
        var offMachinesDict = ProgressDeadlinesFor(offMachineConfigurations, delay);
        
        var merged = processingDict
            .Union(offMachinesDict)
            .Union(exitConfigurations)
            .ToDictionary();
        merged[_inputSequence.InputMachine] = [..merged[_inputSequence.InputMachine], ..partsToSpawn]; 
        
        return new Configuration(merged, oldConfiguration.Time + delay.Amount);
    }

    private Dictionary<Location, Product[]> ProgressDeadlinesFor(IEnumerable<(Machine machine, Product[] parts)> offMachineConfigurations, Delay delay)
    {
        return offMachineConfigurations
            .ToDictionary(
                conf => conf.machine as Location,
                conf=> conf.parts
                    .Select(part => _deadlineCollection.IsCritical(part) 
                        ? part.WithTimeDeadlineProgression(delay.Amount) 
                        : part).ToArray());
    }

    private Dictionary<Location, Product[]> ProgressProcessingAndDeadlinesFor(IEnumerable<(Location location, Product[] parts)> processingLocationConfigurations, Delay delay)
    {
        return processingLocationConfigurations.ToDictionary(e => e.location,
            e => e.parts.Select(part => _deadlineCollection.IsCritical(part) 
                ? part.WithProcessingAndDeadlineTimeProgression(delay.Amount) 
                : part.WithProcessingTimeProgression(delay.Amount))
                .ToArray());
    }

    private static IEnumerable<(Location, Product[] parts)> GetProcessingLocations(Configuration oldConfiguration)
    {
        return oldConfiguration.ConfigurationsOfArms
            .Select(e=>(e.arm as Location, e.parts))
            .Union(oldConfiguration
                .ConfigurationsOfProcessingMachines
                .Select(e=>(e.m as Location, e.parts)));
    }

    private Configuration Place(Placement placement, Configuration oldConfiguration)
    {

        var partsToAdd = placement.Parts.Select(part => part.ToNonProcessed()).ToArray();
        return AddAndRemoveFrom(oldConfiguration, placement.Arm, placement.Machine, placement.Parts, partsToAdd);
    }

    private Configuration PickUp(Pickup pickup, Configuration oldConfiguration)
    {
        var partsToAdd = pickup.Parts.Select(part => part.ToNonProcessed()).ToArray();
        if (pickup.Machine.State is MachineState.Active)
            partsToAdd = partsToAdd.Select(e => e.CreateProgressedCopy()).ToArray();

        return AddAndRemoveFrom(oldConfiguration, pickup.Machine, pickup.Arm, pickup.Parts, partsToAdd);
    }

    private static Configuration AddAndRemoveFrom(Configuration oldConfiguration, Location from, Location to, IEnumerable<Product> toRemove, Product[] toAdd)
    {
        var dict = new Dictionary<Location, Product[]>();
        Remove(oldConfiguration, from, toRemove, dict);
        Add(oldConfiguration, to, toAdd, dict);

        foreach (var location in oldConfiguration.AllLocations.Except([from,to])) 
            dict.Add(location,oldConfiguration.GetConfigurationOf(location).Select(e=>e.Copy()).ToArray());
        
        return new Configuration(dict, oldConfiguration.Time);
    }

    private static void Add(Configuration oldConfiguration, Location to, Product[] toAdd, Dictionary<Location, Product[]> dict)
    {
        var toParts = oldConfiguration.PartsByLocation[to].ToList();
        toParts.AddRange(toAdd);
        dict.Add(to, toParts.ToArray());
    }

    private static void Remove(Configuration oldConfiguration, Location from, IEnumerable<Product> enumerable, Dictionary<Location, Product[]> dict)
    {
        var partsInFrom = oldConfiguration.PartsByLocation[from].Select(e=>e).ToList();
        foreach (var part in enumerable) 
            partsInFrom.Remove(part);
        dict.Add(from, partsInFrom.ToArray());
    }
}