namespace ModelChecker.Search;

internal class OpenList<T>
{
    private readonly PriorityQueue<T, int> _priorityQueue = new();
    private readonly List<(T value, int Cost)> _toAdd = new();
    private bool _shouldReorder;

    public int Count => _toAdd.Count + _priorityQueue.Count;

    public void Enqueue(T value, int cost)
    {
        _toAdd.Add((value, cost));
        _shouldReorder = true;
    }

    public T Peek()
    {
        if (_shouldReorder) Reorder();
        return _priorityQueue.Peek();
    }

    public T Dequeue()
    {
        if (_shouldReorder) Reorder();
        return _priorityQueue.Dequeue();
    }

    private void Reorder()
    {
        _priorityQueue.EnqueueRange(_toAdd);
        _toAdd.Clear();
        _shouldReorder = false;
    }
}