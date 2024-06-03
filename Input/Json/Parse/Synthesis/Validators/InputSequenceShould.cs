using Common.Json.Validation;
using Scheduler.Input.Json.Models;
using Scheduler.Input.Json.Parse.Synthesis.Errors;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

internal class InputSequenceShould
{
    internal static IEnumerable<JsonValidationError> HaveInputMachineDeclared(IEnumerable<string> machines, Models.InputSequenceModel i)
    {
        if (!machines.Contains(i.InputMachine))
            return [new UndeclaredStructure($"{i.InputMachine}", nameof(CatProblemModel.InputSequenceModel), nameof(CatProblemModel.Machines))];

        return [];
    }

    public static IEnumerable<JsonValidationError> HaveValidConstants(InputSequenceModel inputSequenceModel)
    {
        if (inputSequenceModel.BatchSize < 1)
            yield return new InvalidStructure(nameof(inputSequenceModel.BatchSize), "Batch size cannot be smaller than 1");

        if (inputSequenceModel.Sequence.Count % inputSequenceModel.BatchSize != 0)
            yield return new InvalidStructure(nameof(inputSequenceModel.BatchSize), "Batch size must be divisible by input length");

    }
}