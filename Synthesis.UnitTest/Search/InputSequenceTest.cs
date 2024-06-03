using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using ModelChecker.Problem;

namespace Scheduler.ModelChecker.UnitTest.Search;

public class InputSequenceTest
{
    [Fact]
    public void AtTimeZero_ShouldHaveFullSequence()
    {
        var sut = CreateSequence(10, 4, 1);
        sut.InputSequenceAtTime(0).Should().HaveCount(10);
    }

    [Fact]
    public void AtTimeExactlySequence_ShouldHaveSizeBatchMinusTime()
    {
        var sut = CreateSequence(10, 4, 1);
        sut.InputSequenceAtTime(4).Should().HaveCount(9);
    }

    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 20)]
    [InlineData(4, 20)]
    [InlineData(5, 20)]
    public void AfterOneSequence_ShouldHaveFullSequence_MinusBatchSize(int batchSize, int numberOfParts)
    {
        var interval = 4;
        var sut = CreateSequence(numberOfParts, interval, batchSize);
        sut.InputSequenceAtTime(interval).Should().HaveCount(numberOfParts - batchSize);
    }


    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 20)]
    [InlineData(4, 20)]
    [InlineData(5, 20)]
    public void AfterOneInput_AndSome_ShouldHaveFullSequence_MinusBatchSize(int batchSize, int numberOfParts)
    {
        var interval = 4;
        var sut = CreateSequence(numberOfParts, interval, batchSize);
        sut.InputSequenceAtTime(interval + 2).Should().HaveCount(numberOfParts - batchSize);
    }

    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 20)]
    [InlineData(4, 20)]
    [InlineData(5, 20)]
    public void AfterTwoInputs_ShouldHaveFullSequence_MinusBatchSizeTimesTwo(int batchSize, int numberOfParts)
    {
        var interval = 4;
        var sut = CreateSequence(numberOfParts, interval, batchSize);
        sut.InputSequenceAtTime(interval * 2).Should().HaveCount(numberOfParts - batchSize * 2);
    }


    [Theory]
    [InlineData(100, 1)]
    [InlineData(100, 2)]
    [InlineData(100, 5)]
    [InlineData(100, 1000)]
    public void ShouldGiveEmptyList_AfterAllElementsAreEmptied(int numberOfParts, int timeSinceAllInputHappened)
    {
        var timeWhenAllEmptied = numberOfParts;
        var sut = CreateSequence(numberOfParts, 1, 1);
        sut.InputSequenceAtTime(timeWhenAllEmptied + timeSinceAllInputHappened).Should().HaveCount(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(100)]
    public void When_Time_IsExactlyInterval_ButThereIsStillInputLeft_ShouldGiveInterval(int currentTime)
    {
        var sut = CreateSequence(1000, currentTime, 1);
        sut.TimeUntilNextInput(currentTime).Should().Be(currentTime);
    }


    [Theory]
    [InlineData(100, 100)]
    [InlineData(100, 101)]
    [InlineData(1, 11)]
    public void When_NoMoreInput_TimeUntilNextInput_ShouldGive_IntMax(int parts, int partsTaken)
    {
        var sut = CreateSequence(parts, 1, 1);
        var res = sut.TimeUntilNextInput(partsTaken);
        res.Should().Be(int.MaxValue);
    }


    [Theory]
    [InlineData(9, 10, 1)]
    [InlineData(11, 5, 4)]
    [InlineData(13, 5, 2)]
    [InlineData(0, 10, 10)]
    [InlineData(0, 3, 3)]
    public void When_GivenCurrentTime_ShouldAlwaysGiveDistanceToInterval(int currentTime, int interval, int expected)
    {
        var sut = CreateSequence(100, interval, 1);
        var res = sut.TimeUntilNextInput(currentTime);
        res.Should().Be(expected);
    }

    [Fact]
    public void When_WeTake100_Every_1Time_Batch_Of_1_TimeUntilInputDone_Is100()
    {
        var sut = CreateSequence(100, 1, 1);
        sut.DoneTime.Should().Be(100);
    }

    [Fact]
    public void When_WeTake100_Every_1Time_Batch_Of_4_TimeUntilInputDone_Is25()
    {
        var sut = CreateSequence(100, 1, 4);
        sut.DoneTime.Should().Be(25);
    }

    [Fact]
    public void When_WeTake100_Every_1Time_Batch_Of_5_TimeUntilInputDone_Is20()
    {
        var sut = CreateSequence(100, 1, 5);
        sut.DoneTime.Should().Be(20);
    }

    [Fact]
    public void When_WeTake100_Every_3Time_Batch_Of_5_TimeUntilInputDone_Is20()
    {
        var sut = CreateSequence(100, 3, 5);
        sut.DoneTime.Should().Be(60);
    }

    [Fact]
    public void When_WeTake50_Every_2Time_Batch_Of_5_TimeUntilInputDone_Is5()
    {
        var sut = CreateSequence(50, 2, 5);
        sut.DoneTime.Should().Be(20);
    }

    [Theory]
    [InlineData(50, 5, 2)]
    [InlineData(100, 1, 2)]
    [InlineData(100, 5, 1)]
    [InlineData(100, 5, 3)]
    [InlineData(1000, 1, 100)]
    [InlineData(15, 3, 3)]
    [InlineData(15, 5, 5)]
    [InlineData(15, 15, 1)]
    [InlineData(15, 5, 10)]
    [InlineData(15, 5, 7)]
    public void WithNParts_TakingMEvery_TTimeUnits_TimeDoneIs(int parts, int perBatch, int interval)
    {
        var sut = CreateSequence(parts, interval, perBatch);
        sut.DoneTime.Should().Be(parts / perBatch * interval);
    }


    private InputSequence CreateSequence(int numberOfParts, int interval, int batchSize)
    {
        return new InputSequence(Enumerable.Repeat("p1", numberOfParts), interval, batchSize, null,
            new ProtocolList(new Dictionary<string, Protocol>()));
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(10, 20)]
    [InlineData(10, 30)]
    public void WhenTimeIsMultipleOfInterval_IsInputTime_ShouldBeTrue(int interval, int time)
    {
        var seq = CreateSequence(3, interval, 1);
        seq.IsInputTime(time).Should().BeTrue();
    }
    
    [Theory]
    [InlineData(10, 0)]
    [InlineData(10, 1)]
    [InlineData(10, 9)]
    [InlineData(10, 21)]
    [InlineData(10, 19)]
    [InlineData(10, 29)]
    [InlineData(10, 31)]
    public void WhenTimeIsNotMultipleOfInterval_IsInputTime_ShouldBeFalse(int interval, int time)
    {
        var seq = CreateSequence(3, interval, 1);
        seq.IsInputTime(time).Should().BeFalse();
    }
}