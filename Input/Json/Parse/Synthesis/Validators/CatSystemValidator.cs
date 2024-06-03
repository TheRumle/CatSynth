using Common.Json.Validation;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public class CatSystemValidator : Validator<CatSystemModel>
{
    /// <inheritdoc />
    public override Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(CatSystemModel jsonProblemModel)
    {
        var armValidation = new ArmValidator(jsonProblemModel.Machines, jsonProblemModel.Exits).ValidationTasksFor(jsonProblemModel.Arms);
        var deadlineValidation = new DeadlineValidator(jsonProblemModel.Protocol).ValidationTasksFor(jsonProblemModel.Deadline);
        var protocolValidation = new ProtocolValidator(jsonProblemModel.Machines).ValidationTasksFor(jsonProblemModel.Protocol);
        
        var results =  new List<Task<IEnumerable<JsonValidationError>>>(armValidation.Length + deadlineValidation.Length + protocolValidation.Length);
        results.AddRange(armValidation);
        results.AddRange(deadlineValidation);
        results.AddRange(protocolValidation);
        return results.ToArray();
    }
}