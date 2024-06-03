using System.Diagnostics;
using CatConversion.SynthesisVerification;
using Common;
using Common.Results;
using ModelChecker;
using ModelChecker.Problem;

namespace Experiments.ExperimentDriver;

public class SingletonRunner(ScheduleCatVerifier verifier, TimeSpan timeOut)
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="synthesiser"></param>
    /// <param name="problem"></param>e
    public async Task<(Result<Schedule> schedule, string verificationErrors, TimeSpan timeSpent, int numberConfigurations)> Run(ICatSynthesiser synthesiser, SchedulingProblem problem)
    {
        Stopwatch stopwatch = new();
        try
        {
            stopwatch.Start();
            var foundSchedule = await synthesiser.ExecuteAsync(timeOut);
            stopwatch.Stop();
            
            
            if (!foundSchedule.IsSuccess) 
                return (foundSchedule, "", stopwatch.Elapsed, synthesiser.NumberOfConfigurationsExplored);

            try
            {
                var verification = await verifier.CreateAndVerifySchedule(problem, foundSchedule.Value);
        
                if (verification.IsFailure)
                {
                    return (foundSchedule, ErrorStringFormatter.Format(verification.Errors).ToString(), stopwatch.Elapsed,
                        synthesiser.NumberOfConfigurationsExplored);
                }
            }
            catch (IndexOutOfRangeException ignored)
            {}
            
            return (foundSchedule, "", stopwatch.Elapsed, synthesiser.NumberOfConfigurationsExplored);
        }
        catch (OutOfMemoryException e)
        {
            Console.Error.WriteLine(e);
            if (stopwatch.IsRunning) 
                stopwatch.Stop();

            var failure = Result.Failure<Schedule>(new Error("UnexpectedError", e.ToString()));
            return (failure, "", stopwatch.Elapsed, synthesiser.NumberOfConfigurationsExplored);
        }


    }
}