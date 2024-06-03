using System.Collections.Generic;
using System.Linq;
using Common;
using FluentAssertions;
using JetBrains.Annotations;
using ModelChecker.Domain;
using ModelChecker.Domain.Actions;
using ModelChecker.Problem;
using ModelChecker.Search;
using ModelChecker.Semantics;

namespace Scheduler.ModelChecker.UnitTest.Semantics;

[TestSubject(typeof(ActionExecutor))]
public class ActionExecutorTest : SemanticsUnitTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void WhenPickingUpAllParts_IsAddedToArm(int amounts)
    {
        var machine = CreateMachine(1);
        var exit = new Exit("E1");
        var arm = CreateExitArm(2,exit);
        var parts = Enumerable.Repeat(CreateAllNewPart(CreateEmptyProtocolList()), amounts).ToArray();

        var configuration = CreateConfiguration([(arm, []), (machine, parts)]);
        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);
        var pickup = new Pickup(machine, arm, parts);

        var result = actionExecutor.Execute(pickup, configuration);
        result.GetConfigurationOf(machine).Should().BeEmpty();
        result.GetConfigurationOf(arm).Should().AllSatisfy(p => parts.Should().Contain(p));
        result.AllParts.Length.Should().Be(parts.Length);
        result.AllLocations.Should().HaveCount(2);

    }


    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void WhenPickingUpSubsetOfAllParts_IsAddedToArm(int amounts)
    {
        var machine = CreateMachine(1);
        var exit = new Exit("E1");
        var arm = CreateExitArm(2, exit);
        var parts = Enumerable.Repeat(CreateAllNewPart(CreateEmptyProtocolList()), amounts).ToArray();

        var configuration = CreateConfiguration([(arm, []), (machine, parts)]);

        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);

        var pickup = new Pickup(machine, arm, parts.Shuffle().Take(2).ToArray());

        var result = actionExecutor.Execute(pickup, configuration);
        var armConfig = result.GetConfigurationOf(arm);
        armConfig.Length.Should().Be(2);
        armConfig.Should().AllSatisfy(p => parts.Should().Contain(p));

        result.GetConfigurationOf(machine).Length.Should().Be(parts.Length - armConfig.Length);
        result.AllParts.Length.Should().Be(parts.Length);
        result.AllLocations.Should().HaveCount(2);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public void WhenPlacing_AllPartsAreAddedToMachine(int amounts)
    {
        var machine = CreateMachine(1);
        var exit = new Exit("E1");
        var arm = CreateExitArm(2, exit);
        var parts = Enumerable.Repeat(CreateAllNewPart(CreateEmptyProtocolList()), amounts).ToArray();

        var configuration = CreateConfiguration([(arm, parts), (machine, [])]);
        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);

        var placement = new Placement(machine, arm, parts);

        var result = actionExecutor.Execute(placement, configuration);

        result.GetConfigurationOf(machine).Should().BeEquivalentTo(parts);
        result.GetConfigurationOf(arm).Should().BeEmpty();
        result.AllParts.Length.Should().Be(parts.Length);
        result.AllLocations.Should().HaveCount(2);
    }


    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void WhenExitingParts_ArmIsEmpty(int partAmounts)
    {
        var machine = CreateMachine(1);        var exit = new Exit("E1");
        var arm = CreateExitArm(2,exit);
        var part = CreateAllNewPart(CreateEmptyProtocolList());

        var configuration = CreateConfiguration( [(machine, []),(exit,[]),(arm, Enumerable.Repeat(part, partAmounts).ToArray())]);

        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);
        var exitPlacement = new ExitPlacement(arm, exit);


        var result = actionExecutor.Execute(exitPlacement, configuration);
        result.GetConfigurationOf(arm).Should().BeEmpty();
        result.AllParts.Length.Should().Be(partAmounts);
        result.GetConfigurationOf(exit).Length.Should().Be(partAmounts);
        result.AllLocations.Should().HaveCount(3);
    }

    [Fact]
    public void WhenStartingMachine_ShouldSetStateToOn()
    {
        var machine = CreateMachine(1, MachineState.Off);

        var configuration = CreateEmptyConfiguration([machine], 0);

        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);
        var startAction = new MachineStart(machine);


        var result = actionExecutor.Execute(startAction, configuration);
        result.AllLocations.Count().Should().Be(1);
        var newMachine = result.AllLocations.OfType<Machine>().First();
        AssertSameMachineButWithState(machine, newMachine, MachineState.On);
        configuration.Time.Should().Be(result.Time);
    }

    [Fact]
    public void WhenStartingMachine_ShouldSetMachineStateToOn()
    {
        var machine = CreateMachine(1, MachineState.Off);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var part = CreateNewPart(protocol, 10, 10);

        var configuration = CreateConfiguration([(machine, [part])]);
        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);
        var machineStart = new MachineStart(machine);


        var result = actionExecutor.Execute(machineStart, configuration);

        result.AllLocations.Count().Should().Be(1);
        var newMachine = result.AllLocations.OfType<Machine>().First();
        AssertSameMachineButWithState(machine, newMachine, MachineState.On);
    } 



    [Fact]
    public void WhenStoppingMachine_ShouldSetStateToOff()
    {
        var machine = CreateMachine(1, MachineState.On);

        var configuration = CreateEmptyConfiguration([machine], 0);

        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);
        var exit = new MachineStop(machine);


        var result = actionExecutor.Execute(exit, configuration);
        result.AllLocations.Count().Should().Be(1);
        var newMachine = result.AllLocations.OfType<Machine>().First();
        AssertSameMachineButWithState(machine, newMachine, MachineState.Off);
    }

    [Fact]
    public void WhenStoppingMachine_PartShouldUpdateCompletedSteps_ShouldSetStateToOff()
    {
        var machine = CreateMachine(1, MachineState.On);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var part = CreateNewPart(protocol, 10, 10);

        var configuration = CreateConfiguration([(machine, [part])]);
        var actionExecutor = CreateNoDeadlineNoInputActionExecutor(machine);
        var machineStop = new MachineStop(machine);


        var result = actionExecutor.Execute(machineStop, configuration);

        result.AllLocations.Count().Should().Be(1);
        var newMachine = result.AllLocations.OfType<Machine>().First();
        AssertSameMachineButWithState(machine, newMachine, MachineState.Off);
        var updatedPart = result.GetConfigurationOf(newMachine).First();
        updatedPart.CompletedSteps.Should().Be(1);
        updatedPart.RemainingSteps.Should().Be(0);
        updatedPart.IsCompleted.Should().BeTrue();
        updatedPart.PartType.Should().Be(part.PartType);
        updatedPart.TimeProcessed.Should().Be(0);
        updatedPart.UnstableTime.Should().Be(part.UnstableTime);
        result.AllLocations.Should().HaveCount(1);

    }


    [Fact]
    public void IncreasesTimeForNewConfig_Machine()
    {
        var machine = CreateMachine(1);
        var configuration = CreateEmptyConfiguration([machine], 0);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var executor = CreateActionExecutor(CreateNeverReachedDeadline(), CreateEmptyInputSequence(machine, protocol),
            [], [machine]);
        var delay = CreateDelay(10);

        var newConfig = executor.Execute(delay, configuration);
        newConfig.Time.Should().Be(delay.Amount);
        newConfig.AllLocations.Should().HaveCount(1);
    }

    [Fact]
    public void IncreasesTimeForNewConfig_Arm()
    {
        var inputMachine = CreateMachine(1);
        var arm = CreateArm(1);
        var configuration = CreateEmptyConfiguration([arm, inputMachine], 0);
        var protocol = CreateEmptyProtocolList();
        var executor = CreateActionExecutor(CreateNeverReachedDeadline(),
            CreateEmptyInputSequence(inputMachine, protocol), [arm], [inputMachine]);
        var delay = CreateDelay(10);

        var newConfig = executor.Execute(delay, configuration);
        newConfig.Time.Should().Be(delay.Amount);
        newConfig.AllLocations.Should().HaveCount(2);
    }


    [Fact]
    public void Increase_DeadlineTime_ForCriticalParts_WhereMachineIsOff()
    {
        var machine = CreateMachine(1, MachineState.Off);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var part = CreateAllNewPart(protocol);

        var configuration = CreateConfiguration([(machine, [part])]);
        var executor = CreateActionExecutor(
            CreateImmediateDeadline(),
            CreateEmptyInputSequence(machine, protocol),
            [], [machine]);

        var delay = CreateDelay(10);
        var newConfig = executor.Execute(delay, configuration);
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.TimeProcessed.Should().Be(0));
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.UnstableTime.Should().Be(10));
        newConfig.AllLocations.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(MachineState.Active)]
    [InlineData(MachineState.On)]
    public void IncreaseProcessing_AndDeadline_ForCriticalProcessingParts_WhereMachineIs(MachineState state)
    {
        var machine = CreateMachine(1, state);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var part = CreateAllNewPart(protocol);

        var configuration = CreateConfiguration([(machine, [part])]);
        var executor = CreateActionExecutor(
            CreateImmediateDeadline(),
            CreateEmptyInputSequence(machine, protocol),
            [], [machine]);

        var delay = CreateDelay(10);
        var newConfig = executor.Execute(delay, configuration);
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.TimeProcessed.Should().Be(10));
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.UnstableTime.Should().Be(10));
        newConfig.AllLocations.Should().HaveCount(1);
    }

    [Fact]
    public void IncreaseOnlyCriticalWhenMachineIsOff()
    {
        var machine = CreateMachine(1, MachineState.Off);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var part = CreateAllNewPart(protocol);

        var configuration = CreateConfiguration([(machine, [part])]);
        var executor = CreateActionExecutor(
            CreateImmediateDeadline(),
            CreateEmptyInputSequence(machine, protocol),
            [], [machine]);

        var delay = CreateDelay(10);
        var newConfig = executor.Execute(delay, configuration);
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.TimeProcessed.Should().Be(0));
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.UnstableTime.Should().Be(10));
        newConfig.AllParts.Length.Should().Be(1);
        newConfig.AllLocations.Should().HaveCount(1);

    }


    [Fact]
    public void DoesNotIncreaseProcessingTimeWhenMachineIsOff()
    {
        var machine = CreateMachine(1);
        var configuration = CreateEmptyConfiguration([machine], 0);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var executor = CreateActionExecutor(CreateNeverReachedDeadline(), CreateEmptyInputSequence(machine, protocol),
            [], [machine]);
        var delay = CreateDelay(10);

        var newConfig = executor.Execute(delay, configuration);
        newConfig.Time.Should().Be(delay.Amount);
        newConfig.AllLocations.Should().HaveCount(1);

    }

    [Theory]
    [InlineData(MachineState.Active)]
    [InlineData(MachineState.On)]
    public void IncreaseProcessingWhenTimeIs(MachineState state)
    {
        var machine = CreateMachine(1, state);
        var protocol = CreateSingletonProtocolList(machine, 0, 0);
        var part = CreateAllNewPart(protocol);

        var configuration = CreateConfiguration([(machine, [part])]);
        var executor = CreateActionExecutor(
            CreateNeverReachedDeadline(),
            CreateEmptyInputSequence(machine, protocol),
            [], [machine]);

        var delay = CreateDelay(10);
        var newConfig = executor.Execute(delay, configuration);
        newConfig.GetConfigurationOf(machine).Should().AllSatisfy(e => e.TimeProcessed.Should().Be(10));
        newConfig.AllParts.Length.Should().Be(1);
        newConfig.AllLocations.Should().HaveCount(1);
    }


    [Fact]
    public void SpawnsNewParts_WhenDelayIsEqual_InputSequenceInterval()
    {
        var interval = 10;
        var numberOfParts = 1;

        var inputMachine = CreateMachine(1, MachineState.Off);
        var protocol = CreateSingletonProtocolList(inputMachine, 0, 0);
        var inputSequence = CreateInputSequenceSpawningEvery(interval, numberOfParts, inputMachine, protocol);

        var configuration = CreateEmptyConfiguration([inputMachine], 0);

        var executor = CreateActionExecutor(
            CreateNeverReachedDeadline(),
            inputSequence,
            [], [inputMachine]);

        var delay = CreateDelay(interval);

        var newConfig = executor.Execute(delay, configuration);

        var inputMachineParts = newConfig.GetConfigurationOf(inputMachine);
        inputMachineParts.Should().HaveCount(numberOfParts);
        inputMachineParts.First().TimeProcessed.Should().Be(0);
        inputMachineParts.First().UnstableTime.Should().Be(0);
        inputMachineParts.First().CompletedSteps.Should().Be(0);
        newConfig.AllParts.Length.Should().Be(numberOfParts);
        newConfig.AllLocations.Should().HaveCount(1);

    }


    [Fact]
    public void DoesNotSpawnParts_WhenDelayIsJustBelow_InputSequenceInterval()
    {
        var interval = 10;
        var numberOfElements = 1;

        var inputMachine = CreateMachine(1, MachineState.Off);
        var protocol = CreateSingletonProtocolList(inputMachine, 0, 0);
        var inputSequence = CreateInputSequenceSpawningEvery(interval, numberOfElements, inputMachine, protocol);

        var configuration = CreateEmptyConfiguration([inputMachine], 0);

        var executor = CreateActionExecutor(
            CreateNeverReachedDeadline(),
            inputSequence,
            [], [inputMachine]);

        var delay = CreateDelay(interval - 1);

        var newConfig = executor.Execute(delay, configuration);

        var inputMachineParts = newConfig.GetConfigurationOf(inputMachine);
        inputMachineParts.Should().HaveCount(0);
        newConfig.AllParts.Length.Should().Be(0);
        newConfig.AllLocations.Should().HaveCount(1);
    }

    [Fact]
    [Trait("Category","Regression")]
    [Trait("Category","Bugfix")]
    public void WhenPlacingInOffMachineAndDelaying_PartsAndLocationsDoesNotDisappear()
    {
        var productType = "P1";
        var start = Machine.Active(2, "first machine");
        var M1 = new Machine(1, "second machine", MachineState.Off);
        var exit = new Exit("E1");
        var arm = new Arm("Arm", [start, M1], 1, 1, [exit]);
        var protocol = new Protocol([(start, 1, 2), (M1, 1, 1)]);
        ProtocolList list = new ProtocolList(new Dictionary<string, Protocol>
        {
            { PARTTYPE,  protocol}
        });
        
        InputSequence inputSequence = new([productType], 1, 1, start, list);
        
        var startConfig = new Configuration([
            (M1,[new Product(protocol,0, 0, PARTTYPE,1,0)]), 
            (start, [new Product(protocol,0,1,PARTTYPE,0,1)]),
            (arm,[])
        ], 5);

        IActionExecutor executor = CreateActionExecutor(NoDeadlines(), inputSequence, [arm], [M1, start]);
        
        var newConf = executor.Execute(new Delay(1), startConfig);
        newConf.Size.Should().Be(startConfig.Size);
        newConf.AllLocations.Should().BeEquivalentTo(startConfig.AllLocations);
        newConf.AllLocations.Should().HaveCount(3);
    }

    private static void AssertSameMachineButWithState(Machine machine, Machine newMachine, MachineState state)
    {
        newMachine.Name.Should().Be(machine.Name);
        newMachine.Capacity.Should().Be(machine.Capacity);
        newMachine.RequiredAmounts.Should().BeSameAs(machine.RequiredAmounts);
        newMachine.State.Should().Be(state);
    }


}