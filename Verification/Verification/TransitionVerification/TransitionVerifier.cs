using Cat.Verify.Definitions;
using Common;
using Common.Results;

namespace Cat.Verify.Verification.TransitionVerification;

internal sealed class TransitionVerifier(CatContext catContext) : ITransitionVerifier
{
    /// <inheritdoc />
    public Result IsCohesive(Transition transition)
    {
        
        return transition.Operation switch
        {
            Pickup op => Execute(op,(operation,scan)=>scan.OnMachinePickup(transition.Time,operation,transition.C,transition.CPrime)),
            Placement op => Execute(op,(operation,scan)=>scan.OnPlacement(transition.Time,operation,transition.C,transition.CPrime)),
            Start op => Execute(op,(operation,scan)=>scan.OnMachineStart(transition.Time,operation,transition.C,transition.CPrime)),
            Stop op => Execute(op,(operation,scan)=>scan.OnMachineStop(transition.Time,operation,transition.C,transition.CPrime)),
            Arrival op => Execute(op,(operation,scan)=>scan.OnInputOperation(transition.Time,operation,transition.C,transition.CPrime)),
            _ => Result.Failure(new Error($"UNEXPECTED", $"The type of the operation was {transition.Operation.GetType().FullName} which is not a well-defined CAT operation."))
        };
    }

    private Result Execute<T>(T operation, Action<T,TransitionScan> assert )
    {
        var executionScan = new TransitionScan(catContext);
        assert.Invoke(operation,executionScan);
        return executionScan.Errors.Count != 0 ? Result.Failure(executionScan.Errors.ToHashSet().ToArray()) : Result.Success();
    }
}