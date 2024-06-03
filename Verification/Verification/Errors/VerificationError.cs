using Common;

namespace Cat.Verify.Verification.Errors;

public sealed class VerificationError(int time, string description, string rule) : Error($"CAT-Verify",
    $"At time {time}, {rule} is broken.\n\t {description}")
{
    public VerificationError(int time, string description, int rule) : this(time,description, $"Rule {rule}")
    {
        
    }
}
    