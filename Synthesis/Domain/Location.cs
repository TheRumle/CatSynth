namespace ModelChecker.Domain;

public abstract record Location
{
    public readonly int Capacity;
    public readonly string Name;

    protected Location(int capacity, string name)
    {
        Capacity = capacity;
        Name = name;
    }
}