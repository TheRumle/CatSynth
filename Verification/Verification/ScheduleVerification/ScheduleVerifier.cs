using Cat.Verify.Definitions;
using Common.Results;

namespace Cat.Verify.Verification.ScheduleVerification;

internal sealed class ScheduleVerifier : IScheduleVerifier
{
    private readonly CatContext _context;

    internal ScheduleVerifier(CatContext context)
    {
        _context = context;
    }
    
    /// <inheritdoc />
    public Result VerifySchedule(Schedule schedule)
    {
        ScheduleScan scan = new(_context);
        foreach (var (time, action) in schedule.steps)
        {
            if (action is Arrival input) scan.OnInput(time,input);
            if(action is Pickup pi) scan.OnPickup(time,pi);
            if(action is Placement pl) scan.OnPlacement(time,pl);
            if(action is Start machineStart) scan.OnMachineStart(time,machineStart);
            if(action is Stop machineStop) scan.OnMachineStop(time,machineStop);
        }
        return scan.Errors.Any( ) ? Result.Failure(scan.Errors.ToHashSet().ToArray()) : Result.Success();
    }
}