namespace Cat.Verify.Verification.Errors.Execution;

internal sealed class MachineStartErrorsFactory(int time, string m) : VerificationErrorFactory(time)
{
    /// <inheritdoc />
    protected override int RuleNumber => 10;
    
    public VerificationError NotRightAmountOfProducts ()
        => CreateVerificationError('a',$"The number of products in machine '{m}' allows the machine to start");

    public VerificationError NextMustBeMachine(string id, string butWas) =>
        CreateVerificationError('b',$"The next protocol step of all products must be '{m}' but {id} was {butWas}.");
}