using Common.Json.Validation;

namespace Scheduler.Input.Json.Parse.Synthesis.Errors;

public class UndeclaredStructure (string theWrongValue, string theStructureErrorOccuredIn, string whereItShouldHaveBeenDeclared): JsonValidationError
{
    /// <inheritdoc />
    public override string ErrorCategory { get; } = nameof(UndeclaredStructure);

    /// <inheritdoc />
    public override string ToString()
    {
        return
            $@" ""{theStructureErrorOccuredIn}"" contains a value ""{theWrongValue}"" which was not not declared in ""{whereItShouldHaveBeenDeclared}""";
    }
}