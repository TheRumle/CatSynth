using System.Collections.Frozen;
using Combinatorics.Collections;
using Common.Math.Discrete;
using FluentAssertions;

namespace Common.UnitTest.Math.Discrete;

public class CombinerTests
{
    [Fact]
    private void When_Given_4Elements_PartitionedInto3_ShouldHaveSize4()
    {
        var arr = new[]
        {
            1, 2, 3, 4
        };
        Combiner.AllCombinationsOfSize(arr, 3).Should().HaveCount(4);
    }

    [Fact]
    private void When_Given_2Elements_PartitionedInto1_ShouldHaveSize2()
    {
        var arr = new[]
        {
            1, 2
        };
        Combiner.AllCombinationsOfSize(arr, 1).Should().HaveCount(2);
    }

    [Fact]
    private void When_Given_5Elements_PartitionedInto3_ShouldHaveSize10()
    {
        var res = Combiner.AllCombinationsOfSize([1, 2, 3, 4, 5], 3);
        res.Should().HaveCount(10);
    }


    [Fact]
    private void When_Given_MultipleOfSameElements_DoesNotRepeat()
    {
        var res = Combiner.AllCombinationsOfSize([1, 1, 1, 2], 2);

        res.Should().HaveCount(1);
    }


    [Fact]
    private void When_Given_5Elements_PartitionedInto3_ShouldHaveSize10_Combiner()
    {
        int[] res = [1, 2, 3, 4, 5];
        var a = new Combinations<int>(res, 3, GenerateOption.WithoutRepetition);
        a.Should().HaveCount(10);
    }


    [Fact]
    private void When_Given_MultipleOfSameElements_DoesNotRepeat_Combiner()
    {
        int[] res = [1, 1, 1, 2];
        var a = new Combinations<int>(res.ToFrozenSet(), 2, GenerateOption.WithoutRepetition);
        a.Should().HaveCount(1);
    }


    [Fact]
    public void AllKeyCombinations_WhenGiven_ThreeKeys_EachWithTwo_ShouldGive8Combinations()
    {
        IEnumerable<IEnumerable<int>>[] a =
            [[[1, 2, 3], [4, 5, 6]], [[7, 8, 9], [10, 11, 12]], [[-1, -1, -1], [-2, -2, -2]]];
        var result = a.CartesianProduct();
        a.SelectMany(e => e).Should().HaveCount(6);


        result.Should().HaveCount(8);
    }


    [Fact]
    public void WhenChunkingIntoSameSizeAsElementsShouldWork()
    {
        int[] a = [1, 2, 3, 4, 5, 6, 7];

        var result = Combiner.AllCombinationsOfSize(a, 7);
        result.Should().HaveCount(1);
    }
}