using Cat.Verify.Definitions;
using Cat.Verify.Verification.ScheduleVerification;
using Cat.Verify.Verification.TransitionVerification;
using Common.Results;

namespace Cat.Verify.Verification.ExecutionVerification;

internal class ExecutionFeasibleVerifier : IExecutionFeasibleVerifier
{
    private readonly ScheduleVerifier _scheduleVerifier;
    private readonly ExecutionTimeVerifier _executionTimeVerifier;
    private readonly TransitionVerifier _transitionVerifier;

    public ExecutionFeasibleVerifier(CatContext context)
    {
        _scheduleVerifier = new ScheduleVerifier(context);
        _transitionVerifier = new TransitionVerifier(context);
        _executionTimeVerifier = new ExecutionTimeVerifier(context, new ProtocolAnalyser(context));
    }
    
    
    public async Task<Result> IsFeasible(Execution execution)
    {
        var schedule = new Schedule(execution.Steps.Select(e=>(e.Time,e.Operation)));

        var scheduleTask = Task.Run(() => _scheduleVerifier.VerifySchedule(schedule));
        var timelyExecutionTask = Task.Run(() => _executionTimeVerifier.IsTimely(execution));
        var transitionOk = Task.Run(()
            =>   execution.Steps.Select(transition=>_transitionVerifier.IsCohesive(transition))
                .Aggregate((first, second) => first.Collapse(second))).Result;

        await Task.WhenAll(scheduleTask, timelyExecutionTask);
        return scheduleTask.Result.Collapse(timelyExecutionTask.Result).Collapse(transitionOk);
    }
}