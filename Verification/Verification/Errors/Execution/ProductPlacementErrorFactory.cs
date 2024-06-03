using Cat.Verify.Definitions;

namespace Cat.Verify.Verification.Errors.Execution;

internal sealed class ProductPlacementErrorFactory(int time, Placement p) : VerificationErrorFactory(time)
{
    protected override int RuleNumber => 9;

    public VerificationError DoesNotRespectMachineCapacity()
        => CreateVerificationError('a',$"if '{p.Machine}' is a machine, then the capacity of the machine is not violated.");
    
    public VerificationError ProductWasNotInArm(string id)
        => CreateVerificationError('b',$"The product '{id}' is located in the arm {p.Arm}.");
    
    public VerificationError ProductPlacedInActiveMachineWhenNotNextStep(string id)
        => CreateVerificationError('c',$"The next step of product '{id}' is '{p.Machine}' if '{p.Machine}' is an active machine.");
    
    public VerificationError ProductHasNoRemainingProtocolWhenExiting(string id)
        => CreateVerificationError('d',$"The remaining protocol of the product '{id}' is empty if '{p.Machine}' is an exit.");
    
    public VerificationError ProductBecomesTopWhenLeavingSystem(string id)
        => CreateVerificationError('e',$"The product '{id}' leaves the system if '{p.Machine}' is an exit.");
    
    
    public VerificationError TheProductIsPlacedInTheNewLocation()
        => CreateVerificationError('f',$"The product is placed in machine '{p.Machine}'.");
    
    public VerificationError TheArmIsEmptyAfter()
        => CreateVerificationError('v',$"All products disappear from the arm '{p.Arm}'.");
    
}