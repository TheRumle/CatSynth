using Common.Results;

namespace Cat.Verify;

public interface IExecutionFeasibleVerifier
{
    public Task<Result> IsFeasible(Execution execution);
}