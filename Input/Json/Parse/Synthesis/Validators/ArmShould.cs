using Common.Json.Validation;
using Scheduler.Input.Json.Models;
using Scheduler.Input.Json.Parse.Synthesis.Errors;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public static class ArmShould
{
    public static IEnumerable<JsonValidationError> NotHaveUndeclaredLocationInReach(IEnumerable<ArmModel> arms, IEnumerable<MachineModel> machineModels, IEnumerable<string> exits)
    {
        IEnumerable<string> declaredStructures = [..machineModels.Select(e => e.Name), ..exits];
        return arms
            .SelectMany(arm =>
            {
                return arm.Reach
                    .Where(reachableMachine => !declaredStructures.Contains(reachableMachine))
                    .Select(unreacable => new UndeclaredStructure(unreacable, arm.Name, nameof(CatProblemModel.Machines)));
            });

    }

    public static IEnumerable<JsonValidationError> HaveCapacityGreaterThanZero(IEnumerable<ArmModel> models)
    {
        return models.Where(e => e.Capacity <= 0)
            .Select(arm => new InvalidStructure(arm.Name, "The arm must have capacity >=0"));
    }
}