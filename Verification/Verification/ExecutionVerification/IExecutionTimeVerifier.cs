using Common.Results;

namespace Cat.Verify.Verification.ExecutionVerification;

public interface IExecutionTimeVerifier
{
    public Result IsTimely(Execution execution);
}