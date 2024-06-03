namespace Cat.Verify.Verification;

internal sealed class ActionTimeContainer<T> where T : notnull
{
    private List<(int time, T element)> inUse = new();
    public bool Contains(T element) => inUse.Any(e => element.Equals(e.element));

    public void Add(int time, T element)
    {
        inUse.Add((time, element));
    }

    public (int time, T arm)? Get(T element)
    {
        return this.inUse.Find(e => e.element.Equals(element));
    }

    public bool TryGet(T element, out (int armStartTime, T arm) result)
    {
        var hasItem = inUse.Any(e => e.element.Equals(element));
        if (hasItem)
        {
            result = inUse.Find(e => e.element.Equals(element));
            return true;
        }

        result = default;
        return false;
        
    }


    public void Remove(T pArm)
    {
        inUse.RemoveAll(e => e.element.Equals(pArm));
    }
}