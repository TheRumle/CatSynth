using Common.String;

namespace Cat.Verify.Verification.Errors.Execution;

internal sealed class TakeErrorsFactory(int time, string arm, string m) : VerificationErrorFactory(time)
{
    protected override int RuleNumber => 8;

    public VerificationError ProductsAreNotInMachine(IEnumerable<string >productNotInM)
        => CreateVerificationError('a',$"The products '{productNotInM.CommaSeparate()}' are in the machine {m}.");
    
    public   VerificationError ArmIsNotEmpty()
        => CreateVerificationError('b',$"The arm '{arm}' is empty.");
    
    
    public   VerificationError InsufficientArmCapacity()
        => CreateVerificationError('c',$"The arm '{arm}' has sufficient capacity.");
    
    public   VerificationError CompletedStepsNotIncrementedWhenMachineActive(string product )
        => CreateVerificationError('d',$"If {m} is an active machine, then the number of completed steps of the moved product '{product}' is incremented.");
    
    public   VerificationError CompletedStepsIncrementedWhenNotActive(string product )
        => CreateVerificationError('e',$"If {m} is not an active machine, then the number of completed steps of the moved product '{product}' stays the same.");
    
    public   VerificationError ProductNotPlacedInArm(string product, int s)
        => CreateVerificationError("d/e",$"The new configuration of '{product}' should be ({arm}, {s})");
    
    
}