using Common.Json.Validation;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public class ProtocolValidator : Validator<Dictionary<string, List<Step>>>
{
    private readonly IEnumerable<MachineModel> _machineModels;

    public ProtocolValidator(IEnumerable<MachineModel> machineModels)
    {
        _machineModels = machineModels;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(Dictionary<string, List<Step>> jsonModel)
    {
        return BeginValidationsOver(jsonModel, [
            protocol => ProtocolShould.HaveMachinesDeclared(protocol, _machineModels),
            ProtocolShould.HaveMeaningfulConstants
        ]);
    }
}