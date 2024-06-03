using Common.Json.Validation;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public class InputSequenceValidator : Validator<InputSequenceModel>
{
    private readonly IEnumerable<string> _machineModels;

    public InputSequenceValidator(IEnumerable<string> machineModels)
    {
        _machineModels = machineModels;
    }
    
    
    /// <inheritdoc />
    public override Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(InputSequenceModel jsonModel)
    {
        return BeginValidationsOver(jsonModel, [
            i => InputSequenceShould.HaveInputMachineDeclared(_machineModels, i),
            InputSequenceShould.HaveValidConstants
        ]);
    }
}