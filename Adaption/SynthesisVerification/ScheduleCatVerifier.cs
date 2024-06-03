
using Cat.Verify;
using Cat.Verify.Definitions;
using Cat.Verify.Verification;
using Common.Results;
using ModelChecker;
using ModelChecker.Problem;

namespace CatConversion.SynthesisVerification;

/// <summary>
/// Verifies whether a given schedule is CAT-valid for a scheduling problem. 
/// </summary>
/// <param name="schedulingProblem"></param>
/// <param name="schedule"></param>
public class ScheduleCatVerifier
{
    private ExecutionVerifierFactory _verifierFactory = new();
    public async Task<Result> CreateAndVerifySchedule(SchedulingProblem schedulingProblem, Schedule schedule)
    {
        (CatContext context, Execution execution) = ScheduleCatConverter.Convert(schedulingProblem, schedule); ;
        return await _verifierFactory.ConstructVerifier(context).IsFeasible(execution);
    }
    
    public async Task<Result> Verify(CatContext context, Execution execution)
    {
        return await _verifierFactory.ConstructVerifier(context).IsFeasible(execution);
    }
    
    
}