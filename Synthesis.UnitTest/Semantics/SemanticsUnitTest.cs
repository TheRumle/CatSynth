using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics;

namespace Scheduler.ModelChecker.UnitTest.Semantics;

public abstract class SemanticsUnitTest
{
    protected const string PARTTYPE = "P1";
    protected readonly Random _idGenerator = new(47238934);
    protected readonly int StartTime = new Random().Next();


    protected Protocol CreateProtocol(Machine nextStep, int min, int max)
    {
        return new Protocol([new ProtocolStep(nextStep, min, max)]);
    }

    protected Arm CreateArm(int capacity)
    {
        return new Arm("ARM",[], capacity, 4,[]);
    }

    
    protected Arm CreateArmWithId(int capacity, string name)
    {
        return new Arm(name,[], capacity, 4,[]);
    }

    protected Arm CreateArm(int capacity, Machine[] reach)
    {
        return new Arm("ARM",reach, capacity, 4,[]);
    }

    protected Arm CreateExitArm(int capacity, Exit exit)
    {
        return new Arm("ARM",[], capacity, 4, [exit]);
    }

    protected Delay CreateDelay(int amount)
    {
        return new Delay(amount);
    }

    protected DeadlineCollection CreateImmediateDeadline()
    {
        return new DeadlineCollection([(PARTTYPE, 0, int.MaxValue, int.MaxValue)]);
    }

    protected DeadlineCollection CreateNeverReachedDeadline()
    {
        return new DeadlineCollection([(PARTTYPE, int.MaxValue, int.MaxValue, int.MaxValue)]);
    }

    protected ProtocolList CreateProtocolList(Protocol protocol)
    {
        return new ProtocolList(new Dictionary<string, Protocol>
        {
            { PARTTYPE, protocol }
        });
    }

    protected ProtocolList CreateSingletonProtocolList(Machine nextStep, int min, int max)
    {
        return new ProtocolList(new Dictionary<string, Protocol>
        {
            { PARTTYPE, CreateProtocol(nextStep, min, max) }
        });
    }

    protected ProtocolList CreateSingletonProtocolList(int min, int max)
    {
        return CreateSingletonProtocolList(CreateMachine(1), min, max);
    }

    protected ProtocolList CreateEmptyProtocolList()
    {
        return new ProtocolList(new Dictionary<string, Protocol>
        {
            { PARTTYPE, new Protocol(new List<ProtocolStep>()) }
        });
    }

    public IActionExecutor CreateActionExecutor(DeadlineCollection collection, InputSequence inputSequence, Arm[] arms,
        Machine[] machines)
    {
        return new ActionExecutor(collection, inputSequence);
    }


    public IActionExecutor CreateNoDeadlineNoInputActionExecutor(Machine inputMachine)
    {
        var deadline = CreateNeverReachedDeadline();
        return new ActionExecutor(deadline,
            CreateEmptyInputSequence(inputMachine, CreateEmptyProtocolList()));
    }


    protected Product CreateNewPart(ProtocolList protocol, int processingTime, int critTime)
    {
        return new Product(protocol.ProtocolFor(PARTTYPE), processingTime, critTime, PARTTYPE, 0, new Random().Next());
    }
    
    protected Product CreateNewPart(ProtocolList protocol, int processingTime, int critTime, string partType)
    {
        return new Product(protocol.ProtocolFor(PARTTYPE), processingTime, critTime, PARTTYPE, 0, new Random().Next());
    }

    protected Product CreateDonePart(int processingTime)
    {
        return new Product(CreateEmptyProtocolList().ProtocolFor(PARTTYPE), processingTime, 0, PARTTYPE, int.MaxValue, new Random().Next());
    }


    protected InputSequence CreateEmptyInputSequence(Machine inputMachine, ProtocolList list)
    {
        return new InputSequence([], 100000, 1, inputMachine, list);
    }

    protected InputSequence CreateInputSequenceSpawningEvery(int interval, int amount, Machine inputMachine,
        ProtocolList protocolList)
    {
        return new InputSequence(Enumerable.Repeat(PARTTYPE, 1000), interval, amount, inputMachine, protocolList);
    }

    protected Machine CreateMachineWithId(int capacity, int id)
    {
        return new Machine(capacity, id.ToString());
    }

    protected Machine CreateMachine(int capacity)
    {
        return new Machine(capacity, _idGenerator.Next().ToString());
    }

    protected Machine CreateMachine(int capacity, MachineState state)
    {
        return new Machine(capacity, _idGenerator.Next().ToString(), state);
    }

    protected Machine CreateMachine(int capacity, MachineState state, int[] requiredAmounts)
    {
        return new Machine(capacity, _idGenerator.Next().ToString(), requiredAmounts, state);
    }

    protected Configuration CreateConfiguration((Location location, Product[] configuration)[] configurations)
    {
        var dict = configurations.ToDictionary(
            e => e.location,
            e => e.configuration);
        return new Configuration(dict, StartTime);
    }

    protected Configuration CreateConfiguration((Location location, Product[] parts)[] configurations, int time)
    {
        var dict = configurations.ToDictionary(
            e => e.location,
            e => e.parts);
        return new Configuration(dict, time);
    }

    protected Configuration CreateEmptyConfiguration(Location[] location, int time)
    {
        var dict = location.ToDictionary(
            e => e,
            e => new Product[] { });

        return new Configuration(dict, time);
    }

    protected static Product CreateAllNewPart(ProtocolList protocol)
    {
        return new Product(protocol.ProtocolFor(PARTTYPE), 0, 0, PARTTYPE, 0, new Random().Next());
    }


    protected Product CreateNotDonePart(int min, int max)
    {
        var l = CreateSingletonProtocolList(min, max);
        return new Product(l.ProtocolFor(PARTTYPE), min - 1, 0, PARTTYPE, 0, new Random().Next());
    }

    protected Product CreatePartWithNextStepBeing(Machine machine, int minProcessingTime)
    {
        var p = new Protocol([new ProtocolStep(machine, minProcessingTime, minProcessingTime + 1)]);
        return new Product(p, minProcessingTime, 0, PARTTYPE, 0, new Random().Next());
    }

    protected Product CreateDonePartWithNextStepBeing(Machine machine, int minProcessingTime, int maxProcessingTime)
    {
        var p = new Protocol([new ProtocolStep(machine, minProcessingTime, maxProcessingTime)]);
        return new Product(p, minProcessingTime, 0, PARTTYPE, 0, new Random().Next());
    }

    protected Product CreateNotDonePartWithNextStepBeing(Machine machine, int minProcessingTime, int maxProcessingTime)
    {
        var p = new Protocol([new ProtocolStep(machine, minProcessingTime, maxProcessingTime)]);
        return new Product(p, minProcessingTime - 1, 0, PARTTYPE, 0, new Random().Next());
    }

    protected T[] Repeat<T>(T element, int times)
    {
        return Enumerable.Repeat(element, times).ToArray();
    }

    public void AssertBy<T>(IEnumerable<T> actions, params Action<IEnumerable<T>>[] assertions) where T : SystemAction
    {
        using (new AssertionScope())
        {
            var systemActions = actions as T[] ?? actions.ToArray();
            foreach (var assertion in assertions)
                assertion.Invoke(systemActions);
        }
    }
    public DeadlineCollection NoDeadlines()
    {
        return new DeadlineCollection([(PARTTYPE, int.MaxValue, int.MaxValue, int.MaxValue)]);
    }
}