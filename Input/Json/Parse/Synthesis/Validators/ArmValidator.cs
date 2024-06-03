using Common.Json.Validation;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public class ArmValidator : Validator<IEnumerable<ArmModel>>
{
    private readonly IEnumerable<MachineModel> _machineModels;
    private readonly IEnumerable<string> _exits;

    public ArmValidator(IEnumerable<MachineModel> machineModels, IEnumerable<string> exits)
    {
        _machineModels = machineModels;
        _exits = exits;
    }
    
    /// <inheritdoc />
    public override Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(IEnumerable<ArmModel> jsonModel)
    {   
        return BeginValidationsOver(jsonModel, [
            arms=>ArmShould.NotHaveUndeclaredLocationInReach(arms, _machineModels, _exits),
            ArmShould.HaveCapacityGreaterThanZero
        ]);

    }
}
