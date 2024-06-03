namespace Experiments.ExperimentDriver;

/// <summary>
/// 
/// </summary>
/// <param name="algorithmKey">For instance, "CatStar-RPTT"</param>
/// <param name="TimeSpent"></param>
/// <param name="NumberOfConfigurations"></param>
public record CsvRow(
    string algorithmKey,
    string System,
    string ProblemInstance,
    double TimeSpent,
    int NumberOfConfigurations,
    int MakeSpan,
    bool Success,
    string FailReason)
{
    public static char Separator = ';';
    public static string Header = $"Key{Separator}{Separator}System{Separator}Instance{Separator}Time{Separator}NumberOfConfigurations{Separator}MakeSpan{Separator}FoundSolution{Separator}\nFailReason";
    
    public string ToCsvRow()
    {
        return $"{algorithmKey}{Separator}{System}{Separator}{ProblemInstance}{Separator}{TimeSpent}{Separator}{NumberOfConfigurations}{Separator}{MakeSpan}{Separator}{Success.ToString()}{Separator}{FailReason}\n";
    }
    
    
}