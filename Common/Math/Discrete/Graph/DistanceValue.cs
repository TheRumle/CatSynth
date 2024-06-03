namespace Common.Math.Discrete.Graph;

public struct DistanceValue<T>(T from, T to, float distance)
{
    public readonly T From = from;
    public readonly T To = to;
    public readonly float Distance = distance;

    public static DistanceValue<T> SelfDistance(T value)
    {
        return new DistanceValue<T>(value, value, 0);
    }

    public static DistanceValue<T> MaxDistance(T from, T to)
    {
        return new DistanceValue<T>(from, to, float.MaxValue);
    } 
}