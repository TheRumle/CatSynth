using Common.Json.Validation;
using Scheduler.Input.Json.Models;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public class DeadlineValidator : Validator<List<DeadlineIndex>>
{
    private readonly Dictionary<string, List<Step>> _protocols;

    public DeadlineValidator(Dictionary<string, List<Step>>  protocols)
    {
        _protocols = protocols;
    }
    
    /// <inheritdoc />
    public override Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(List<DeadlineIndex> jsonModel)
    {
        return BeginValidationsOver(jsonModel, [
            _ => DeadlineShould.NotHaveUndefinedParts(jsonModel, _protocols),
            _ => DeadlineShould.HaveValidIndexes(jsonModel, _protocols)
        ]);
    }
}