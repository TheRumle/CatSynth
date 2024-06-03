using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using ModelChecker.Domain;
using ModelChecker.Factory;
using ModelChecker.Problem;
using ModelChecker.Semantics;

namespace Scheduler.ModelChecker.UnitTest.Semantics;

[TestSubject(typeof(PossibleDelayComputer))]
public class PossibleDelayComputerTest : SemanticsUnitTest
{ 
    private static readonly string Part = "P1";
    private static readonly Machine TestMachine = new(10, "FirstMachine", MachineState.On);
    private readonly Machine _inputMachine = new(10, "SecondMachine", MachineState.Off);
    private readonly PartFactory _partFactory = new([Part]);
    private readonly ProtocolFactory _protocolFactory = new([TestMachine]);


    [Theory]
    [InlineData(0, 3, 1, 2)] //Delay to reach 2,3, by amounts 1,2
    [InlineData(1, 3, 1, 2)] //Delay to reach age 2,3, by amounts 1, 2
    [InlineData(1, 10, 1, 9)] //Delay to reach age 2,3,..,10, by amounts 1,2,3,4,5,6,7,8,9
    [InlineData(1, 10, 6, 4)] //..., by amounts 1,2,3,4
    [InlineData(1, 10, 5, 5)] //Delay to reach age 6,7,8,9,10, by amounts 1,2,3,4,5
    [InlineData(1, 10, 10, 0)] //No delay possible
    public void WhenAgeBetweenInvariant_ShouldGiveExpectedNumberOfPossibleDelays(int min, int max, int processingTime,
        int expected)
    {
        var legalValues = Enumerable.Range(1, expected);
        
        
        var sut = GetComputer(SetupNotWithinDeadline());
        var startConfiguration = CreateConfiguration(min, max, processingTime);
        var possibleDelays = sut.ComputePossibleDelays(startConfiguration);
        possibleDelays.Length.Should().Be(expected);
        
        possibleDelays.Should().AllSatisfy(e => legalValues.Should().Contain(e.Amount));
    }

    [Theory]
    [InlineData(5, 10, 0, 6)] //Delay to reach 5,6,7,8,9,10, amounts 5,6,7,8,9,10
    [InlineData(5, 10, 1, 6)] //Delay to reach 5,6,7,8,9,10, amounts 4,5,6,7,8,9
    [InlineData(5, 10, 2, 6)] //Delay to reach 5,6,7,8,9,10, amounts 3,4,5,6,7,8
    [InlineData(5, 10, 4, 6)] //Delay to reach 5,6,7,8,9,10
    [InlineData(10, 10, 0, 1)] //Delay to reach 10
    [InlineData(5, 10, 5, 5)] //Delay to reach 6,7,8,9,10
    public void WhenAgeUnderInvariant_ShouldGiveExpectedNumberOfPossibleDelays(int min, int max, int processingTime, int expected)
    {
        var legalValues =  Enumerable.Range(min-processingTime, max-min+1);
        
        var sut = GetComputer(SetupNotWithinDeadline());
        var startConfiguration = CreateConfiguration(min, max, processingTime);

        var possibleActions = sut.ComputePossibleDelays(startConfiguration);
        possibleActions.Length.Should().Be(expected);
        possibleActions.Should().AllSatisfy(e => legalValues.Should().Contain(e.Amount));

    }


    [Fact(Skip = "If we reach a configuration where nothing can happen at any point, delay does not make sense.")]
    public void WhenIdleInOffMachine_WithNoDeadline_ReturnsZeroDelayPossible()
    {
        var machine = new Machine(1, "testMachine", MachineState.Off);
        var sut = GetComputer(SetupNotWithinDeadline(), machine);
        var startConfiguration = CreateConfiguration(5, 10, 0, machine);
        var possibleActions = sut.ComputePossibleDelays(startConfiguration);
        possibleActions.Length.Should().Be(0);
    }

    [Fact]
    public void OldestPart_ShouldDictate_MaxAndMinDelay()
    {
        var machine = new Machine(1, "testMachine", MachineState.On);
        var l = CreateSingletonProtocolList(5, 10);
        var p1 = this.CreateNewPart(l,2,0,PARTTYPE);
        var p2 = this.CreateNewPart(l,4,0,PARTTYPE);
        
        var legalValues =  Enumerable.Range(5-4, 10-5+1);
        
        var sut = GetComputer(SetupNotWithinDeadline(), machine);
        var startConfiguration = base.CreateConfiguration([(machine, [p1,p2])]);
        var possibleActions = sut.ComputePossibleDelays(startConfiguration);
        possibleActions.Length.Should().Be(6);
        possibleActions.Should().AllSatisfy(e => legalValues.Should().Contain(e.Amount));

    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(10)]
    public void InputSequenceDictatesMax(int time)
    {
        var interval = 10;
        
        var machine = new Machine(2, "testMachine", MachineState.Off);
        var inputSequence = CreateInputSequenceSpawningEvery(interval, 2, machine, CreateEmptyProtocolList());
        
        var sut = GetComputer(SetupNotWithinDeadline(), inputSequence);
        
        var startConfiguration = base.CreateConfiguration([(machine,[])],time);
        var possibleActions = sut.ComputePossibleDelays(startConfiguration);
        possibleActions.Length.Should().Be(1);
    }


    [Fact]
    public void DoesNotThrowWhen_ProcessingTimeExceeded()
    {
        
        var deadline = new DeadlineCollection([("P1", 0,1, 1000)]);
        var fM = new Machine(1, "first", MachineState.Active);
        var sM = new Machine(1, "second", MachineState.On);
        var prot = new Protocol([(fM, 1, 20), (sM, 3, 5)]);
        var protList = new ProtocolList(new Dictionary<string, Protocol>()
        {
            { "P1", prot }
        });
        
        var inputSequence = new InputSequence(["P1", "P1"], 6, 1, fM,protList );
        
        var sut = new PossibleDelayComputer(deadline, inputSequence);
        var p1 = new Product(protList.ProtocolFor("P1"),0,2,"P1",1, 1);
        
        //PROCESSING TIME EXCEED PROTOCOL
        var p2 = new Product(protList.ProtocolFor("P1"),50,2,"P1",1, 2);
        var exit = new Exit("exit");
        var arm = new Arm("arm", [fM, sM], 1, 6, [exit]);
        var config = new Configuration([(exit,[]),(fM,[]),(arm,[p1]), (sM, [p2])], 16); 


        var delays = sut.ComputePossibleDelays(config);
        delays.Length.Should().Be(0);//No delay possible
    }
    
    [Fact]
    public void EmptySequence_OneDelayAvailable()
    {
        
        var deadline = new DeadlineCollection([("P1", 0,1, 1000)]);
        var fM = new Machine(1, "first", MachineState.Active);
        var sM = new Machine(1, "second", MachineState.On);
        var prot = new Protocol([(fM, 1, 20), (sM, 3, 5)]);
        var protList = new ProtocolList(new Dictionary<string, Protocol>()
        {
            { "P1", prot }
        });
        
        var inputSequence = new InputSequence(["P1", "P1"], 6, 1, fM,protList );
        
        var sut = new PossibleDelayComputer(deadline, inputSequence);
        var exit = new Exit("exit");
        var arm = new Arm("arm", [fM, sM], 1, 6, [exit]);
        var config = new Configuration([(exit,[]),(fM,[]),(arm,[]), (sM, [])], 0); 


        var delays = sut.ComputePossibleDelays(config);
        delays.Length.Should().Be(1);//No delay possible
    }

    
    
    [Fact]
    public void WhenMinDelayGreaterThanDeadline_DeadlineShouldLimitTheDelay()
    {
        var deadline = new DeadlineCollection([("P1", 0,1, 120)]);
        var h = new Machine(1, "first", MachineState.Active);
        var c = new Machine(4, "second", [1,2,4],MachineState.Off);
        var prot = new Protocol([(h, 50, 60), (c, 30, 30)]);
        var protList = new ProtocolList(new Dictionary<string, Protocol>()
        {
            { "P1", prot }
        });
        
        var inputSequence = new InputSequence(["P1", "P1", "P1"], 100, 1, h,protList );
        
        var sut = new PossibleDelayComputer(deadline, inputSequence);
        var p1 = new Product(protList.ProtocolFor("P1"),0,0,"P1",2, 0);
        var p2 = new Product(protList.ProtocolFor("P1"),0,100,"P1",1, 1);
        var p3 = new Product(protList.ProtocolFor("P1"),0,0,"P1",0, 2);
        
        var exit = new Exit("exit");
        var arm = new Arm("arm", [h, c], 1, 6, [exit]);
        
        
        var config = new Configuration([(exit,[p1]),(c,[p2]),(arm,[]), (h, [p3])], 300);


        var delay = sut.ComputePossibleDelays(config).Should().AllSatisfy(e=>e.Amount.Should().BeLessThan(21));
    }

    private DeadlineCollection SetupWithinDeadline()
    {
        return new DeadlineCollection([(Part, 0, int.MaxValue, 0)]);
    }

    private DeadlineCollection SetupNotWithinDeadline()
    {
        return new DeadlineCollection([(Part, int.MaxValue, int.MaxValue, 0)]);
    }

    private PossibleDelayComputer GetComputer(DeadlineCollection deadlineCollection)
    {
        return new PossibleDelayComputer(deadlineCollection,
            new InputSequence(Enumerable.Repeat("p1", 1), int.MaxValue, 1, _inputMachine,
                new ProtocolList(new Dictionary<string, Protocol>())));
    }

    private PossibleDelayComputer GetComputer(DeadlineCollection deadlineCollection, Machine machine)
    {
        return new PossibleDelayComputer(deadlineCollection,
            new InputSequence(Enumerable.Repeat("p1", 1), int.MaxValue, 1, machine,
                new ProtocolList(new Dictionary<string, Protocol>())));
    }
    
    private PossibleDelayComputer GetComputer(DeadlineCollection deadlineCollection, InputSequence inputSequence)
    {
        return new PossibleDelayComputer(deadlineCollection, inputSequence);
    }

    private Configuration CreateConfiguration(int min, int max, int age, Machine machine)
    {
        var journey = _protocolFactory.CreateJourney([(TestMachine, min, max)]);
        var part = _partFactory.NewPartWithProcessingTime(journey, Part, age);
        return new Configuration([(machine, [part]), (_inputMachine, [])], 0);
    }

    private Configuration CreateConfiguration(int min, int max, int age)
    {
        var journey = _protocolFactory.CreateJourney([(TestMachine, min, max)]);
        var part = _partFactory.NewPartWithProcessingTime(journey, Part, age);
        return new Configuration([(TestMachine, [part]), (_inputMachine, [])], 0);
    }
    
    
}