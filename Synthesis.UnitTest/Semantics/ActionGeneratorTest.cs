using System.Linq;
using Common;
using FluentAssertions;
using JetBrains.Annotations;
using ModelChecker.Domain;
using ModelChecker.Semantics;

namespace Scheduler.ModelChecker.UnitTest.Semantics;

[TestSubject(typeof(ActionGenerator))]
public class ActionGeneratorTest : SemanticsUnitTest
{
    private (Machine machine, Arm arm, Configuration configuration) SingletonArmConfiguration(Product p1,
        MachineState machineState)
    {
        var machine = CreateMachine(10, machineState);
        var arm = CreateArm(3, [machine]);
        var configuration = CreateConfiguration([
            (machine, []),
            (arm, [p1])
        ]);
        return (machine, arm, configuration);
    }

    private (Machine machine, Arm arm, Configuration configuration) SingletonMachineConfiguration(Product p1)
    {
        var machine = CreateMachine(10, MachineState.On);
        var arm = CreateArm(3, [machine]);
        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [])
        ]);
        return (machine, arm, configuration);
    }

    private (Machine machine, Arm arm, Configuration configuration) MachineConfigurationWithParts(Product[] parts)
    {
        var machine = CreateMachine(10, MachineState.On);
        var arm = CreateArm(3, [machine]);
        var configuration = CreateConfiguration([
            (machine, parts),
            (arm, [])
        ]);
        return (machine, arm, configuration);
    }

    private (Machine machine, Arm arm, Configuration configuration) ArmConfigurationWithParts(Product[] parts)
    {
        var machine = CreateMachine(10, MachineState.On);
        var arm = CreateArm(3, [machine]);
        var configuration = CreateConfiguration([
            (machine, []),
            (arm, parts)
        ]);
        return (machine, arm, configuration);
    }

    #region Exit

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(7)]
    [InlineData(13)]
    public void WhenDonePartsCanReachExit_ShouldGenerate_OneExitAction_RegardlessOfNumberOfParts(int numberOfParts)
    {
        var exit = new Exit("E1");
        var arm = CreateExitArm(numberOfParts,exit);
        var part = CreateDonePart(arm.Time);

        var configuration = CreateConfiguration([(arm, Enumerable.Repeat(part, numberOfParts).ToArray())]);

        var generatedActions = ActionGenerator.CreateExits(configuration);

        AssertBy(generatedActions,
            e => e.Count().Should().Be(1),
            e => e.Should().AllSatisfy(exit => exit.Arm.Should().Be(arm))
        );
    }

    [Fact]
    public void IfOnePartIsNotDone_ShouldNotCreateExit()
    {
        var exit = new Exit("E1");
        var arm = CreateExitArm(10,exit);
        var (done, notDone) = (CreateDonePart(arm.Time), CreateNotDonePart(10, 20));

        var configuration = CreateConfiguration(
        [
            (arm,
                Enumerable.Repeat(done, 10).Concat([notDone]).Shuffle().ToArray())
        ]);

        var generatedActions = ActionGenerator.CreateExits(configuration);

        AssertBy(generatedActions,
            e => e.Should().BeEmpty()
        );
    }

    #endregion

    #region Start

    [Fact]
    public void WhenMachineHasOnlyOnePart_ButTwoAreRequired_ShouldNotStart()
    {
        var minProcessingTime = 1;
        var machine = CreateMachine(10, MachineState.Off, [2]);
        var part = CreatePartWithNextStepBeing(machine, minProcessingTime);
        var configuration = CreateConfiguration([(machine, [part])]);
        var actions = ActionGenerator.CreateMachineStarts(configuration);
        actions.Should().HaveCount(0);
    }
    
    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(13)]
    public void WhenMachineHasNumberOfParts_InRequiredAmountOfParts_CanStart(int amountOfParts)
    {
        var minProcessingTime = 1;
        var machine = CreateMachine(10, MachineState.Off, [2, 4, 6, 8, 13]);
        var parts = Repeat(CreatePartWithNextStepBeing(machine, minProcessingTime), amountOfParts);
        var configuration = CreateConfiguration([(machine, parts)]);

        var actions = ActionGenerator.CreateMachineStarts(configuration);

        AssertBy(actions,
            e => e.Count().Should().Be(1),
            e => e.First().Machine.Should().Be(machine)
        );
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(13)]
    public void WhenMachineHasNumberOfParts_NotInRequiredAmountOfParts_CannotStart(int amountOfParts)
    {
        var machine = CreateMachine(10, MachineState.Off, [2, 4, 6, 8, 13]);
        var parts = Repeat(CreatePartWithNextStepBeing(machine, 1), amountOfParts - 1);

        var configuration = CreateConfiguration([(machine, parts)]);

        var actions = ActionGenerator.CreateMachineStarts(configuration);

        AssertBy(actions,
            e => e.Count().Should().Be(0)
        );
    }

    [Fact]
    public void WhenHavingMultipleParts_ButOnePartDoesNotGoInMachine_ShouldNotStart()
    {
        var otherMachine = CreateMachine(10);
        var machine = CreateMachine(10, MachineState.Off, [2]);
        Product[] parts = [CreatePartWithNextStepBeing(machine, 1), CreatePartWithNextStepBeing(otherMachine, 1)];

        var configuration = CreateConfiguration([(machine, parts), (otherMachine, [])]);

        var actions = ActionGenerator.CreateMachineStarts(configuration);

        AssertBy(actions,
            e => e.Count().Should().Be(0)
        );
    }

    [Fact]
    public void WhenSinglePartDoesNotGoInMachine_ShouldNotStart()
    {
        var otherMachine = CreateMachine(10);
        var machine = CreateMachine(10, MachineState.Off);
        Product[] parts = [CreatePartWithNextStepBeing(otherMachine, 1)];

        var configuration = CreateConfiguration([(machine, parts), (otherMachine, [])]);

        var actions = ActionGenerator.CreateMachineStarts(configuration);

        AssertBy(actions,
            e => e.Count().Should().Be(0)
        );
    }

    #endregion

    #region End

    [Fact]
    public void IfOnePartHasNotProcessedEnough_CannotEnd()
    {
        var machine = CreateMachine(10, MachineState.On);
        var configuration = CreateConfiguration(
        [
            (
                location: machine,
                configuration:
                [
                    CreateDonePartWithNextStepBeing(machine, 10, 20),
                    CreateNotDonePartWithNextStepBeing(machine, 10, 20)
                ])
        ]);

        var stops = ActionGenerator.CreateMachineStops(configuration);

        AssertBy(stops,
            e => e.Should().BeEmpty()
        );
    }

    [Fact]
    public void IfBothPartHasProcessedEnough_CanEnd()
    {
        var machine = CreateMachine(10, MachineState.On);
        var configuration = CreateConfiguration(
        [
            (
                location: machine,
                configuration:
                [
                    CreateDonePartWithNextStepBeing(machine, 10, 20),
                    CreateDonePartWithNextStepBeing(machine, 10, 20)
                ])
        ]);

        var stops = ActionGenerator.CreateMachineStops(configuration);

        AssertBy(stops,
            e => e.Should().HaveCount(1)
        );
    }

    #endregion

    #region PartPickUp

    [Fact]
    public void WhenArmHoldsAnotherPart_CannotPickUp()
    {
        var machine = CreateMachine(10, MachineState.Off);
        var arm = CreateArm(10, [machine]);
        var (p1, p2) = (CreateDonePart(arm.Time), CreateNotDonePart(10, 20));

        var configuration = CreateConfiguration([
            (arm, [p2]),
            (machine, [p1])
        ]);

        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }


    [Fact]
    public void WhenMachineIsOn_CannotPickAnyUpInArm()
    {
        var machine = CreateMachine(10, MachineState.On);
        var arm = CreateArm(10, [machine]);
        var p1 = CreateDonePart(10);

        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [])
        ]);

        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }


    [Fact]
    public void WhenArmCannotReachMachine_CannotPickAnyUpInArm()
    {
        var machine = CreateMachine(10, MachineState.Off);
        var arm = CreateArm(10, []);
        var p1 = CreateDonePart(10);

        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [])
        ]);

        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }

    [Fact]
    public void WhenArmCanReachMachine_CanPickUp()
    {
        var machine = CreateMachine(10, MachineState.Off);
        var arm = CreateArm(10, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 12);

        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [])
        ]);


        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(1)
        );
    }

    [Fact]
    public void WhenMachineIsActive_CannotPlaceInMachine_WhenNextStepInProtocolIsNotMachine()
    {
        var m1 = CreateMachine(10, MachineState.Active);
        var m2 = CreateMachine(10, MachineState.Off);
        var arm = CreateArm(10, [m1]);
        var p1 = CreateDonePartWithNextStepBeing(m2, 10, 11);
        
        var configuration = CreateConfiguration([
            (m1,[]),
            (m2,[]),
            (arm, [p1])
        ]);
        
        var actions = ActionGenerator.CreatePartPlacements(configuration);
        actions.Where(e=>e.Machine.Equals(m1)).Should().HaveCount(0);
    }
    
    
    
    
    [Fact]
    public void WhenMachineIsActive_CanPickUpInArm_WhenPartIsOldEnough()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(10, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);

        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [])
        ]);


        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(1)
        );
    }

    [Fact]
    public void WhenMachineIsActive_CannotPickUpInArm_WhenPartIsNotOldEnough()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(10, [machine]);
        var p1 = CreateNotDonePart(11, 12);

        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [])
        ]);


        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }


    [Fact]
    public void WhenTwoOfSameParts_CanBePickedUp_ShouldGiveOneCombinationOnly()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(1, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);

        var configuration = CreateConfiguration([
            (machine, [p1, p1]),
            (arm, [])
        ]);


        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(1),
            e => e.Should().AllSatisfy(pickup => pickup.Machine.Should().Be(machine)),
            e => e.Should().AllSatisfy(pickup => pickup.Arm.Should().Be(arm))
        );
    }

    [Fact]
    public void WhenTwoParts_AndCapacityOne_CanBePickedUp_ShouldGive2CombinationOnly()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(1, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);

        var configuration = CreateConfiguration([
            (machine, [p1, CreateDonePartWithNextStepBeing(machine, 11, 22)]),
            (arm, [])
        ]);


        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(2),
            e => e.Should().AllSatisfy(pickup => pickup.Machine.Should().Be(machine)),
            e => e.Should().AllSatisfy(pickup => pickup.Arm.Should().Be(arm))
        );
    }

    [Fact]
    public void WhenFourParts_CanBePickedUp_ShouldGive34Combinations()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(3, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);
        var p2 = CreateDonePartWithNextStepBeing(machine, 11, 23);
        var p3 = CreateDonePartWithNextStepBeing(machine, 12, 11);
        var p4 = CreateDonePartWithNextStepBeing(machine, 13, 18);

        var configuration = CreateConfiguration([
            (machine, [p1, p2, p3, p4]),
            (arm, [])
        ]);


        var actions = ActionGenerator.CreatePartPickUps(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(34),
            e => e.Should().AllSatisfy(pickup => pickup.Machine.Should().Be(machine)),
            e => e.Should().AllSatisfy(pickup => pickup.Arm.Should().Be(arm))
        );
    }

    #endregion

    #region PartPlacement

    [Fact]
    public void WhenMachineIsOff_CanPlaceInMachine()
    {
        var machine = CreateMachine(1, MachineState.Off);
        var arm = CreateArm(1, [machine]);
        var p1 = CreateDonePart(arm.Time);

        var actions = ActionGenerator.CreatePartPlacements(CreateConfiguration([(machine,[]), (arm,[p1])]));

        AssertBy(actions,
            e => e.Should().HaveCount(1),
            e => e.First().Machine.Should().Be(machine),
            e => e.First().Arm.Should().Be(arm)
        );
    }

    [Fact]
    public void WhenMachineIsOn_CannotPlaceInMachine()
    {
        var p1 = CreateAllNewPart(CreateEmptyProtocolList());
        var (_, _, configuration) = SingletonArmConfiguration(p1, MachineState.On);

        var actions = ActionGenerator.CreatePartPlacements(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }


    [Fact]
    public void WhenMachineIsActive_CannotPlaceInMachine_IfPartsNextJourney_IsOtherMachine()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(3, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(CreateMachine(5), 10, 11);

        var configuration = CreateConfiguration([
            (machine, []),
            (arm, [p1])
        ]);

        var actions = ActionGenerator.CreatePartPlacements(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }

    [Fact]
    public void WhenMachineIsActive_AndOnePartDoesNotHaveNextStepInThatMachine_CannotPlaceInMachine()
    {
        var machine = CreateMachine(10, MachineState.Active);
        var arm = CreateArm(3, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(CreateMachine(5), 10, 11);
        var p2 = CreateDonePartWithNextStepBeing(machine, 11, 11);

        var configuration = CreateConfiguration([
            (machine, []),
            (arm, [p1, p2])
        ]);

        var actions = ActionGenerator.CreatePartPlacements(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }

    [Theory]
    [InlineData(MachineState.Off)]
    [InlineData(MachineState.Active)]
    public void WhenMachineDoesNotHaveCapacityDueToCurrentConfiguration_CannotPlaceInMachine(MachineState state)
    {
        var machine = CreateMachine(1, state);
        var arm = CreateArm(3, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);
        var p2 = CreateDonePartWithNextStepBeing(machine, 11, 11);

        var configuration = CreateConfiguration([
            (machine, [p1]),
            (arm, [p2])
        ]);

        var actions = ActionGenerator.CreatePartPlacements(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }

    [Theory]
    [InlineData(MachineState.Off)]
    [InlineData(MachineState.Active)]
    public void WhenMachineDoesNotHaveCapacityDueToMaxCapacity_CannotPlaceInMachine(MachineState state)
    {
        var machine = CreateMachine(1, state);
        var arm = CreateArm(3, [machine]);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);

        var configuration = CreateConfiguration([
            (machine, []),
            (arm, [p1, p1])
        ]);

        var actions = ActionGenerator.CreatePartPlacements(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }

    [Theory]
    [InlineData(MachineState.Off)]
    [InlineData(MachineState.Active)]
    public void WhenArmCannotReachMachine_CannotPlaceInMachine(MachineState state)
    {
        var machine = CreateMachine(10, state);
        var arm = CreateArm(3, []);
        var p1 = CreateDonePartWithNextStepBeing(machine, 10, 11);

        var configuration = CreateConfiguration([
            (machine, []),
            (arm, [p1])
        ]);

        var actions = ActionGenerator.CreatePartPlacements(configuration);

        AssertBy(actions,
            e => e.Should().HaveCount(0)
        );
    }

    #endregion
}