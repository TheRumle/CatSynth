using Common.Results;

namespace Cat.Verify.Verification.TransitionVerification;

internal interface ITransitionVerifier
{
    public Result IsCohesive(Transition transition);
}