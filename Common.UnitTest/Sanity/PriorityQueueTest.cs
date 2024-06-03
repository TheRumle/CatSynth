using FluentAssertions;

namespace Common.UnitTest.Sanity;

public class PriorityQueueTest
{
    private PriorityQueue<int, float> _queue = new();
    
    [Fact]
    public void LargerValueIsFirst()
    {
        _queue.Enqueue(1,1);
        _queue.Enqueue(2,2);
        _queue.Peek().Should().Be(1);

    }
    
}