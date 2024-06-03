using Cat.Verify.Definitions;
using Cat.Verify.Verification.Errors;
using Cat.Verify.Verification.Errors.Execution;

namespace Cat.Verify.Verification.TransitionVerification;

internal sealed class TransitionScan
{
    private readonly CatContext _context;
    private readonly ProtocolAnalyser _protocolAnalyser;
    public List<VerificationError> Errors = new();

    public TransitionScan(CatContext context)
    {
        _context = context;
        _protocolAnalyser = new (context);
    }
    


    public void OnMachineStart(int time, Start machineStart, Configuration c, Configuration cPrime)
    {
        MachineStartErrorsFactory errorsFactory = new (time, machineStart.Machine);
        var requiredParts = _context.RequiredParts[machineStart.Machine].ToArray();
        var requiredPartsOkay = requiredParts.Contains(c.ContentOf(machineStart.Machine).Count());
        
        if (!requiredPartsOkay) 
            Errors.Add(errorsFactory.NotRightAmountOfProducts());

        foreach (var id in c.ContentOf(machineStart.Machine))
        {
            var (m, minTime, maxTime) = _protocolAnalyser.Next(id, c);
            if (!m.Equals(machineStart.Machine))
                Errors.Add(errorsFactory.NextMustBeMachine(id, _protocolAnalyser.Next(id, c).machine));
        }

        AssertNoProductsMove(time,c,cPrime, machineStart);
        AssertNoProductsUpdated(time,c,cPrime, machineStart);
        
    }
    
    public void OnMachineStop(int time, Stop machineStop, Configuration c, Configuration cPrime)
    {
        MachineStopErrorsFactory errorsFactory = new(time);
        foreach (var id in c.ContentOf(machineStop.Machine))
        {
            if (cPrime[id].stepsCompleted != c[id].stepsCompleted + 1)
            {
                Errors.Add(errorsFactory.TheNumberOfCompletedStepsIsNotIncremented(id));
            }
        }
        AssertNoProductsMove(time,c,cPrime,machineStop);
        AssertNoOtherProductsUpdated(time,c,cPrime,c.ContentOf(machineStop.Machine),machineStop);
    }

    public void OnMachinePickup(int time, Pickup pickup, Configuration c, Configuration cPrime)
    {
        TakeErrorsFactory takeErrors = new(time, pickup.Arm, pickup.Machine);
        var inMachine = c.ContentOf(pickup.Machine);
        if (!inMachine.ToHashSet().IsSupersetOf(pickup.ProductIds)) 
            Errors.Add(takeErrors.ProductsAreNotInMachine(pickup.ProductIds.Except(inMachine)));
        
        if(c.ContentOf(pickup.Arm).Any())
            Errors.Add(takeErrors.ArmIsNotEmpty());
        
        if (pickup.ProductIds.Count() > _context.Capacity[pickup.Arm])
            Errors.Add(takeErrors.InsufficientArmCapacity());

        foreach (var id in pickup.ProductIds) 
        {
            if (cPrime[id].location != pickup.Arm)
            {
                Errors.Add(takeErrors.ProductNotPlacedInArm(id, cPrime[id].stepsCompleted));
            }
            
            if (_context.ActiveMachines.Contains(pickup.Machine) && 
                cPrime[id].stepsCompleted != c[id].stepsCompleted+1)
            {
                Errors.Add(takeErrors.CompletedStepsNotIncrementedWhenMachineActive(id));
            }
            else if (_context.Machines.Contains(pickup.Machine) && cPrime[id].stepsCompleted != c[id].stepsCompleted)
            {
                Errors.Add(takeErrors.CompletedStepsIncrementedWhenNotActive(id));
            }
        }
        
        AssertNoOtherProductMove(time, c, cPrime, pickup);
    }

    public void OnPlacement(int time, Placement placement, Configuration c, Configuration cPrime)
    {
        ProductPlacementErrorFactory errorFactory = new(time, placement);
        var m = placement.Machine;
        var a = placement.Arm;
        var pt = placement.ProductIds;
        var ids = pt as string[] ?? pt.ToArray();
        foreach (var id in pt)
        {
            //capacity holds
            if (_context.Machines.Contains(m) && _context.Capacity[m] < c.ContentOf(m).Count() + ids.Length)
            {
                Errors.Add(errorFactory.DoesNotRespectMachineCapacity());
            } 
            //product is in arm
            if (c[id].location != a)
            {
                Errors.Add(errorFactory.ProductWasNotInArm(id));
            }
            
            // next is the active machine
            if (_protocolAnalyser.Next(id, c).machine != m && _context.ActiveMachines.Contains(m))
            {
                Errors.Add(errorFactory.ProductPlacedInActiveMachineWhenNotNextStep(id));
            }
            // the protocol is completed if it is an exit
            else if (_context.Protocol[id].Skip(c[id].stepsCompleted).Any() && _context.Exits.Contains(m))
            {
                Errors.Add(errorFactory.ProductHasNoRemainingProtocolWhenExiting(id));
            }
            // the location is updated and the steps completed remain unchanged
            if (cPrime[id].location != m || cPrime[id].stepsCompleted != c[id].stepsCompleted && _context.Machines.Contains(m))
            {
                Errors.Add(errorFactory.TheProductIsPlacedInTheNewLocation());
            }
            // if it is an exit, the product is set to Bot
            if (cPrime[id] != Configuration.Bot && _context.Exits.Contains(m))
            {
                Errors.Add(errorFactory.ProductBecomesTopWhenLeavingSystem(id));
            }
        }

        AssertNoOtherProductMove(time, c, cPrime, placement);
        AssertNoProductsUpdated(time, c, cPrime, placement);
    }
    
    public void OnInputOperation(int time, Arrival operation, Configuration c, Configuration cPrime)
    {
        ArrivalErrorsFactory arrivalErrorsFactory = new(time);
        var m = operation.Machine;
        var values = operation.ProductIds;
        var pt = values as string[] ?? values.ToArray();
        foreach (var id in pt)
        {
            if (_context.Capacity[m] < c.ContentOf(m).Count() + pt.Length)
            {
                Errors.Add(arrivalErrorsFactory.CapacityExceeded(operation.Machine));
            }

            if (cPrime[id].location != m || cPrime[id].stepsCompleted != 0)
            {
                Errors.Add(arrivalErrorsFactory.ProductHasInvalidConfiguration(id));
            }
            
            if (c[id] != Configuration.Bot)
            {
                Errors.Add(arrivalErrorsFactory.ProductAlreadyInSystem(id));
            }
        }
        
        AssertNoOtherProductMove(time, c, cPrime, operation);
    }
    
    
    private void AssertNoOtherProductMove(int time, Configuration c, Configuration cPrime, IProductInvolvedOperation operation )
    {
        foreach (var id in _context.ProductIds.Except(operation.ProductIds))
        {
            AssertProductHasMoved(time, c, cPrime, operation, id);
            AssertProductHasUpdated(time, c, cPrime, operation, id);

        }
    }

    private void AssertProductHasUpdated(int time, Configuration c, Configuration cPrime, ICatOperation operation, string id)
    {
        var (_, s)= c[id];
        var (_,sPrime)= cPrime[id];
        if (!sPrime.Equals(s))
        {
            Errors.Add(new VerificationError(time,
                $"{operation.ToPaperFormattedString()} should not change the steps completed of product '{id}'.",
                "Action execution global"));
        }
    }

    private void AssertProductHasMoved(int time, Configuration c, Configuration cPrime, ICatOperation operation,
        string id)
    {
        var (l, _)= c[id];
        var (lPrime, _)= cPrime[id];
        if (!lPrime.Equals(l))
        {
            Errors.Add(new VerificationError(time,
                $"{operation.ToPaperFormattedString()} should not change the location of product '{id}'.",
                "Action execution global"));
        }
    }

    private void AssertNoProductsUpdated(int time, Configuration c, Configuration cPrime, ICatOperation operation )
    {
        foreach (var id in _context.ProductIds)
        {
            AssertProductHasUpdated(time, c, cPrime, operation, id);
        }
    }
    
    
    private void AssertNoOtherProductsUpdated(int time, Configuration c, Configuration cPrime, IEnumerable<string> pt, ICatOperation operation )
    {
        foreach (var id in _context.ProductIds.Except(pt))
        {
            var (_, s)= c[id];
            var (_,sPrime)= cPrime[id];
            if (!sPrime.Equals(s))
            {
                Errors.Add(new VerificationError(time,
                    $"{operation.ToPaperFormattedString()} should not change the steps completed of product '{id}'.",
                    "Action execution global"));
            }
        }
    }

    private void AssertNoProductsMove(int time, Configuration c, Configuration cPrime, ICatOperation operation )
    {
        foreach (var id in _context.ProductIds)
        {
            AssertProductHasMoved(time, c, cPrime, operation, id);
        }
    }
}