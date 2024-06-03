using System.Globalization;
using Common;
using Common.String;
using ModelChecker;
using ModelChecker.Problem;
using ModelChecker.Search;

namespace Experiments.ExperimentDriver;

[Serializable]
public record ExperimentResult(
    string ProblemName,
    string Algorithm,
    string HeuristicName,
    int Makespan,
    double SecondsToExecute,
    int NumberConfigurations,
    double KbUsed,
    bool FoundSolution,
    bool HasErrors)
{
    
    public static ExperimentResult NoSchedule(SchedulingProblem problem, ICatSynthesiser algorithm,
        ISearchHeuristic heuristic, TimeSpan time, double memoryUsed)
    {
        return Create(problem, algorithm, int.MaxValue, time, heuristic, memoryUsed, false,false);
    }

    public static ExperimentResult Successful(SchedulingProblem problem, ICatSynthesiser algorithm,
        ISearchHeuristic heuristic,
        TimeSpan time, double memoryUsed, Schedule schedule, bool hasErrs)
    {
        return Create(problem, algorithm, schedule.TotalMakespan, time, heuristic, memoryUsed, true, hasErrs);
    }

    private static ExperimentResult Create(SchedulingProblem problem, ICatSynthesiser algorithm,
        int makeSpan, TimeSpan elapsed, ISearchHeuristic heuristic, double kbUsed, bool foundSchedule, bool hasErrors)
    {
        return new ExperimentResult(problem.SystemName, algorithm.GetType().Name!, heuristic.PaperName,
            makeSpan, elapsed.TotalSeconds, algorithm.NumberOfConfigurationsExplored, kbUsed, foundSchedule, hasErrors);
    }

    public static string ToHeader(char separator)
    {
        IEnumerable<string> fields = [nameof(ProblemName), nameof(Algorithm), nameof(HeuristicName), nameof(Makespan), "time" ,"ExploredConfigurations", "KbUsed", "VerificationErrors", "FoundSolution" ];
        return fields.CharSeparate(separator);
    }

    public string ToCsvRow(char separator)
    {

        IEnumerable<string> s =
        [
            ProblemName, this.Algorithm, this.HeuristicName, this.Makespan.ToString(), SecondsToExecute.ToString(CultureInfo.InvariantCulture),
            this.NumberConfigurations.ToString(), this.KbUsed.ToString(CultureInfo.InvariantCulture),
            this.HasErrors ? "true" : "false", this.FoundSolution ? "true" : "false"
        ];

        return s.CharSeparate(separator);
    }
}