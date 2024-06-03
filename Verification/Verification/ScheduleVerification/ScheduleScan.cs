using Cat.Verify.Definitions;
using Cat.Verify.Verification.Errors;
using Common;

namespace Cat.Verify.Verification.ScheduleVerification;

internal sealed class ScheduleScan
{
    private readonly CatContext _context;

    public ScheduleScan(CatContext context)
    {
        _context = context;
    }
    
    private ActionTimeContainer<string> armsInUse = new();
    private ActionTimeContainer<string> machinesInUse = new();
    private List<Error> errors = new();
    public IReadOnlyList<Error> Errors => errors.AsReadOnly();
    
    public void OnPickup(int time, Pickup p)
    {
        if (armsInUse.Contains(p.Arm)) AddArmAlreadyInUse(time,p);
        if (machinesInUse.Contains(p.Machine)) AddMachineAlreadyInUse(time, p);
        
        armsInUse.Add(time, p.Arm);
    }
    
    public void OnPlacement(int time, Placement p)
    {
        if (machinesInUse.Contains(p.Machine)) AddMachineAlreadyInUse(time, p);
        if (!armsInUse.Contains(p.Arm)) ArmMoveNotStarted(time, p);

        if (armsInUse.TryGet(p.Arm, out var tuple) &&
            time - tuple.armStartTime != _context.ArmTimes[tuple.arm]) AddArmTimeViolated(time, p);

        armsInUse.Remove(p.Arm);
    }


    public void OnMachineStart(int time, Start p)
    {
        if (machinesInUse.Contains(p.Machine)) AddMachineAlreadyInUse(time, p);
        machinesInUse.Add(time,p.Machine);
    }
    
    public void OnMachineStop(int time, Stop p)
    {
        if (!machinesInUse.Contains(p.Machine)) AddMachineNotStarted(time,p);
        machinesInUse.Remove(p.Machine);
    }

    private void AddMachineNotStarted(int time, Stop stop)
    {
        errors.Add(new VerificationError(time,$"{stop.ToPaperFormattedString()} cannot occur before an {new Start(stop.Machine).ToPaperFormattedString()}!", 2));
    }
    
    private void AddArmAlreadyInUse(int time, IArmOperation p)
    {
        errors.Add(new VerificationError(time,$"{p.ToPaperFormattedString()} cannot occur because {p.Arm} is already in use.", 3));
    }
    
    private void AddMachineAlreadyInUse(int time, IMachineOperation p)
    {
        errors.Add(new VerificationError(time,$"{p.ToPaperFormattedString()} cannot occur because machine '{p.Machine}' is already in use",2));
    }
    
    private void AddArmTimeViolated(int time, Placement placement)
    {
        errors.Add(new VerificationError(time,$"{placement.ToPaperFormattedString()} violates the time constraint for {placement.Arm}!", 3));
    }

    private void ArmMoveNotStarted(int time, Placement placement)
    {
        errors.Add(new VerificationError(time,$"{placement.ToPaperFormattedString()} cannot happen before a {new Pickup(placement.ProductIds, "_", placement.Arm).ToPaperFormattedString()} operation", 3));
    }

    public void OnInput(int time, Arrival input)
    {
        //NOOP, input can occur at any time
    }
}