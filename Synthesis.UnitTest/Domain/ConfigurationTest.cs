using System.Collections.Generic;
using FluentAssertions;
using JetBrains.Annotations;
using ModelChecker.Domain;
using ModelChecker.Problem;
using Scheduler.ModelChecker.UnitTest.Semantics;

namespace Scheduler.ModelChecker.UnitTest.Domain;

[TestSubject(typeof(Configuration))]
public class ConfigurationTest : SemanticsUnitTest
{

    [Fact]
    public void EqualityDoesNotConsiderTime()
    {
        var armOne = CreateArmWithId(4,"arm");
        var first  = CreateEmptyConfiguration([armOne], 2);
        var second = CreateEmptyConfiguration([armOne], 3);

        first.Equals(second).Should().Be(true);
    }

    [Fact]
    public void MachineStateAffectsHasKey()
    {
        var mOff = new Machine(1, "m1", [1, 2, 3], MachineState.Off);
        var mOn = mOff.StartedVersion();
        var protocol = new Protocol([(mOff, 2,1)]);
        var product = new Product(protocol, 0, 0, "p1", 0, 1);
        Configuration c1 = new Configuration([(mOff, [product])], 0);
        Configuration c2 = new Configuration([(mOn, [product])], 0);

        c1.GetHashCode().Should().NotBe(c2.GetHashCode());
    }
    
    
    
    [Fact]
    public void EqualityConsidering_Arms_ArmValueEquality()
    {
        var armOne = CreateArm(4);
        var armTwo = CreateArm(4);
        var first  = CreateEmptyConfiguration([armOne], 2);
        var second = CreateEmptyConfiguration([armTwo], 3);

        first.Equals(second).Should().Be(true);
    }
    
    [Fact]
    public void EqualityConsidering_Machines_MachineValueEquality()
    {
        var mOne = CreateMachineWithId(4,1);
        var mTwo = CreateMachineWithId(4,1);
        mOne.Should().BeEquivalentTo(mTwo);
        var first  = CreateEmptyConfiguration([mOne], 2);
        var second = CreateEmptyConfiguration([mTwo], 3);

        first.Equals(second).Should().Be(true);
    }
    
    [Fact]
    public void EqualityConsidering_Machines_MachineValueEquality_ConsidersId()
    {
        var mOne = new Machine(10,"first");
        var mTwo = new Machine(10,"second");
        mOne.Should().NotBeEquivalentTo(mTwo);
        
        var first  = CreateEmptyConfiguration([mOne], 2);
        var second = CreateEmptyConfiguration([mTwo], 3);
        Assert.Throws<KeyNotFoundException>(() => first.Equals(second).Should().Be(false));
    }

    [Fact]
    public void PriorityList_NotSeenBeforeShouldBeFalse()
    {
        Dictionary<Configuration, float> costSoFar = new();
        var mOne = new Machine(10,"first");
        var first  = CreateEmptyConfiguration([mOne], 2);
        costSoFar.TryGetValue(first, out var previousNextCost).Should().Be(false);
    }
    
    [Fact]
    public void PriorityList_SeenBefore_ShouldBeTrue()
    {
        var mOne = new Machine(10,"second");
        var first  = CreateEmptyConfiguration([mOne], 2);
        Dictionary<Configuration, float> costSoFar = new();
        costSoFar[first] = 1f;
        
        var second  = CreateEmptyConfiguration([mOne], 4);
        costSoFar.TryGetValue(second, out var previousNextCost).Should().Be(true);
        previousNextCost.Should().Be(1f);
    }

}