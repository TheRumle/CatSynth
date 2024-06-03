using Cat.Verify.Definitions;

namespace Cat.Verify.Verification.TransitionVerification;

public sealed record Transition(Configuration C, int Time, ICatOperation Operation, Configuration CPrime);