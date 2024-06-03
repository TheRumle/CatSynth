using System.Text;
using ModelChecker.Domain.Actions;

namespace ModelChecker.Domain;

public readonly record struct Configuration
{
    /// <inheritdoc />
    public bool Equals(Configuration other)
    {
        foreach (var (loc, parts) in _partsByLocation)
        {
            var otherParts = other.PartsByLocation[loc];
            if (parts.Length != otherParts.Length)
                return false;

            if (otherParts.Any(otherPart => !parts.Contains(otherPart)))
                return false;

            if (parts.Any(thisPart => !otherParts.Contains(thisPart)))
                return false;

            if (loc is Machine m)
            {
                if (m.State != other._machinesByName[m.Name].State)
                    return false;
            }

            
        }

        return true;
    }

    private readonly Dictionary<Location, Product[]> _partsByLocation = new();
    public readonly Product[] AllParts;
    public readonly int Time;
    public IReadOnlyDictionary<Location, Product[]> PartsByLocation => _partsByLocation.AsReadOnly();
    public readonly int Size;
    private readonly Dictionary<string,Machine> _machinesByName; 

    public readonly IEnumerable<(Arm arm, Product[] parts)> ConfigurationsOfArms;
    public readonly IEnumerable<(Machine machine, Product[] parts)> ConfigurationsOfMachines;
    public readonly IEnumerable<(Machine m, Product[] parts)> ConfigurationsOfProcessingMachines;

    public Configuration(Dictionary<Location, Product[]> dict, int time)
    {
        _partsByLocation = dict;
        AllParts = GetAllParts(dict);  
        Time = time;
        Size = AllParts.Length;
        ConfigurationsOfArms = GetArmConfigurations(dict);
        ConfigurationsOfMachines = GetMachineConfigurations(dict);
        ConfigurationsOfProcessingMachines = GetProcessingMachineConfigurations(dict);
        _machinesByName = GetMachines(ConfigurationsOfMachines);
    }

    private Dictionary<string, Machine> GetMachines(IEnumerable<(Machine machine, Product[] parts)> configurationsOfMachines)
    {
        var dict = new Dictionary<string,Machine>();
        foreach (var (m,p) in configurationsOfMachines)
        {
            dict.Add(m.Name, m);
        }

        return dict;
    }

    private static Product[] GetAllParts(Dictionary<Location, Product[]> dict)
    {
        int totalPartsCount = dict.Sum(kvp => kvp.Value.Length);
        var allPartsArray = new Product[totalPartsCount];
        int index = 0;
        foreach (var partsArray in dict.Values)
        {
            Array.Copy(partsArray, 0, allPartsArray, index, partsArray.Length);
            index += partsArray.Length;
        }
        return allPartsArray;
    }

    private static IEnumerable<(Arm arm, Product[] parts)> GetArmConfigurations(Dictionary<Location, Product[]> dict)
    {
        foreach (var kvp in dict)
        {
            if (kvp.Key is Arm arm)
            {
                yield return (arm, kvp.Value);
            }
        }
    }

    private static IEnumerable<(Machine machine, Product[] parts)> GetMachineConfigurations(Dictionary<Location, Product[]> dict)
    {
        foreach (var kvp in dict)
        {
            if (kvp.Key is Machine machine)
            {
                yield return (machine, kvp.Value);
            }
        }
    }

    private static IEnumerable<(Machine m, Product[] parts)> GetProcessingMachineConfigurations(Dictionary<Location, Product[]> dict)
    {
        foreach (var (key, value) in dict)
        {
            if (key is Machine machine && machine.State is MachineState.Active or MachineState.On)
            {
                yield return (machine, value);
            }
        }
    }

    public Configuration((Location location, IEnumerable<Product> parts)[] dict, int time):this(dict.ToDictionary(e => e.location, e => e.parts.ToArray()), time)
    { }
    
    public IEnumerable<Location> AllLocations => _partsByLocation.Keys;

    public Product[] GetConfigurationOf(Location location)
    {
        return _partsByLocation[location];
    }

    public int SizeOf(Location location)
    {
        return _partsByLocation[location].Length;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        StringBuilder bob = new();
        foreach (var kvp in _partsByLocation.OrderBy(e=>e.Key.Name))
        {
            bob.Append($"{kvp.Key.Name}: {string.Join(", ", kvp.Value.Select(e => e.Id.ToString()))}\t\t");
        }

        return bob.ToString();
    }
    


    /// <inheritdoc />
    public override int GetHashCode()
    {
        int hash = 17;
        
        foreach (var pair in _partsByLocation)
        {
            hash = hash * 23 + pair.Key.GetHashCode();
            foreach (var part in pair.Value)
            {
                hash = hash * 23 + part.GetHashCode();
            }
        }

        foreach (var loc in _partsByLocation.Keys)
        {
            if (loc is Machine m)
                hash = hash * 43 * HashCode.Combine(m.Name.GetHashCode(), m.State.GetHashCode(), m.Capacity.GetHashCode());
        }
        
        return hash;
    }

    public MachineState GetStateOf(Machine m)
    {
        return _machinesByName[m.Name].State;
    }
}