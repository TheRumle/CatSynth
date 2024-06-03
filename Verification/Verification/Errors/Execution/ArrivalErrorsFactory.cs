namespace Cat.Verify.Verification.Errors.Execution;

internal sealed class ArrivalErrorsFactory(int time) : VerificationErrorFactory(time)
{
    protected override int RuleNumber => 7;

    public   VerificationError CapacityExceeded(string machine)
        => CreateVerificationError('a',$"The arrival of the products does not exceed the capacity of the machine '{machine}'.");
    
    public   VerificationError ProductHasInvalidConfiguration(string product)
        => CreateVerificationError('b',$"The arrived product '{product}' have not completed any steps of their protocol.");
    
    
    public   VerificationError ProductAlreadyInSystem(string product)
        => CreateVerificationError('c',$"The product '{product}' is not in the system yet.");
    
}