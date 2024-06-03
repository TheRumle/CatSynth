namespace Cat.Verify.Verification.Errors.Execution;

internal sealed class MachineStopErrorsFactory(int time) : VerificationErrorFactory(time)
{
    /// <inheritdoc />
    protected override int RuleNumber => 11;
    
    public VerificationError TheNumberOfCompletedStepsIsNotIncremented(string id)
        => CreateVerificationError('a',$"The number of completed steps of product {id} is incremented.");
}