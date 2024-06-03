using Common.Json.Validation;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public class CatProblemValidator : Validator<CatProblemModel>
{
    /// <inheritdoc />
    public override Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(CatProblemModel jsonProblemModel)
    {
        Task<IEnumerable<JsonValidationError>>[] errors = new CatSystemValidator().ValidationTasksFor(new CatSystemModel()
        {
            Arms = jsonProblemModel.Arms,
            Machines = jsonProblemModel.Machines,
            Deadline = jsonProblemModel.Deadline,
            Exits = jsonProblemModel.Exits,
            Protocol = jsonProblemModel.Protocol,
            SystemName = jsonProblemModel.SystemName
        });
 
        var inputSequenceValidation = new InputSequenceValidator(jsonProblemModel.Machines.Select(e=>e.Name)).ValidationTasksFor(jsonProblemModel.InputSequenceModel);
        
        var results =  new List<Task<IEnumerable<JsonValidationError>>>();
        results.AddRange(errors);
        results.AddRange(inputSequenceValidation);
        return results.ToArray();
    }
}