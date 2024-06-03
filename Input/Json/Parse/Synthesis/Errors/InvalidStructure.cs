using Common.Json.Validation;
using Common.String;

namespace Scheduler.Input.Json.Parse.Synthesis.Errors;

public class InvalidStructure(string structure, string explanation) : JsonValidationError
{
    /// <inheritdoc />
    public override string ErrorCategory => nameof(InvalidStructure);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{structure} is invalid. Explanation: {explanation.FirstCharToUpper()}.";
    }
}
