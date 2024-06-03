namespace Common.Math.Discrete.Graph;

public static class FloydWarshallBuilder
{
    public static ShortestDistance<TElements> CreateGraphLookup<TElements>(Dictionary<(TElements,TElements), float> vertices) where TElements : notnull
    {
        var elements = vertices.Keys.SelectMany(pair => new[] { pair.Item1, pair.Item2 }).Distinct().ToList();
        foreach (var k in elements)
        {
            foreach (var i in elements)
            {
                foreach (var j in elements)
                {
                    if (vertices.ContainsKey((i, k)) && vertices.ContainsKey((k, j)))
                    {
                        float ikWeight = vertices[(i, k)];
                        float kjWeight = vertices[(k, j)];
                        if (!vertices.ContainsKey((i, j)) || vertices[(i, j)] > ikWeight + kjWeight) 
                            vertices[(i, j)] = ikWeight + kjWeight;
                    }
                }
            }
        }
        
        return new ShortestDistance<TElements>(vertices);

    }
}