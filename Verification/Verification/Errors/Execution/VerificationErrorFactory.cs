namespace Cat.Verify.Verification.Errors.Execution;

internal abstract class VerificationErrorFactory(int time)
{
    protected abstract int RuleNumber { get; }
    
    protected VerificationError CreateVerificationError(char subRule, string description)
    {
        return new VerificationError(time, description, $"Rule {RuleNumber}.{subRule}");
    }
    
    protected VerificationError CreateVerificationError(string subRule, string description)
    {
        return new VerificationError(time, description, $"Rule {RuleNumber}.{subRule}");
    }
}