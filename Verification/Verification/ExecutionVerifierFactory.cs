using Cat.Verify.Definitions;
using Cat.Verify.Verification.ExecutionVerification;

namespace Cat.Verify.Verification;

public sealed class ExecutionVerifierFactory
{
    public IExecutionFeasibleVerifier ConstructVerifier(CatContext context)
    {
        return new ExecutionFeasibleVerifier(context);
    }
}