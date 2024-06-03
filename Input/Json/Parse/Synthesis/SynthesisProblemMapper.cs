using Common.Json;
using Common.Json.Validation;
using Common.Results;
using ModelChecker.Domain;
using ModelChecker.Factory;
using ModelChecker.Problem;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis;

public class SynthesisProblemMapper(MachineFactory machineFactory)
{

    public SchedulingProblem ConstructSchedulingProblem(CatProblemModel catProblem)
    {
        var machines = catProblem.Machines.Select(e =>
            (machine: machineFactory.CreateMachine(e.Capacity, e.Name, e.RequiredParts, e.State), machineName: e.Name)).ToArray();
        
        var exits = catProblem.Exits.Select(e=>new Exit(e));
        var arms = catProblem.Arms.Select(armModel => ConstructArm(machines, exits, armModel));
        var protocolList = CreateProtocolList(catProblem.Protocol, machines);
        var deadlineCollection = new DeadlineCollection(catProblem.Deadline.Select(e => (e.Product, e.Start, e.End, e.Time)));
        var inputSequence = ConstructInputSequence(catProblem, protocolList, machines);
        var emptyConfig = CreateEmptyConfiguration(machines, arms, exits);
        return new SchedulingProblem(catProblem.SystemName, emptyConfig, deadlineCollection, inputSequence, protocolList);
    }

    private Configuration CreateEmptyConfiguration((Machine machine, string machineName)[] machineByName,
        IEnumerable<Arm> arms, IEnumerable<Exit> exits)
    {
        var configurationByLocation = machineByName
            .Select(e => e.machine)
            .Union<Location>(arms)
            .Union(exits)
            .ToDictionary(
                machine => machine,
                _ => new Product[] { }
            );

        return new Configuration(configurationByLocation,0);
    }

    private static InputSequence ConstructInputSequence(CatProblemModel catProblem, ProtocolList protocolList,
        IEnumerable<(Machine machine, string machineName)> machines)
    {
        return new InputSequence(catProblem.InputSequenceModel.Sequence, catProblem.InputSequenceModel.Every, catProblem.InputSequenceModel.BatchSize, machines.First(e=>e.machineName == catProblem.InputSequenceModel.InputMachine).machine, protocolList);
    }


    private ProtocolList CreateProtocolList(Dictionary<string, List<Step>> modelProtocol,
        IEnumerable<(Machine machine, string machineName)> machines)
    {
        var protocolPerProduct =  modelProtocol
            .Select(e => (product: e.Key, protocol: CreateProtocol(e.Value, machines)))
            .ToDictionary(e=>e.product, e=>e.protocol);
        
        return new ProtocolList(protocolPerProduct);
    }

    private static Protocol CreateProtocol(List<Step> steps, IEnumerable<(Machine machine, string machineName)> machines)
    {
        var result = steps.Select(step =>
            new ProtocolStep(machines.First(byName => byName.machineName == step.Machine).machine, step.MinTime, step.MaxTime));
        return new Protocol(result);
    }

    private static Arm ConstructArm(IEnumerable<(Machine machine, string machineName)> machines, IEnumerable<Exit> exits, ArmModel armModel)
    {
        var reachableMachines = machines.Where(m => armModel.Reach.Contains(m.machineName)).Select(e=>e.machine);
        var reachableExits = exits.Where(e => armModel.Reach.Contains(e.Name));
        return new Arm(armModel.Name,reachableMachines.ToArray(),armModel.Capacity, armModel.Time,reachableExits.ToArray());
    }
}