using Common.Json.Validation;
using Scheduler.Input.Json.Models;
using Scheduler.Input.Json.Parse.Synthesis.Errors;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public static class DeadlineShould
{
    public static IEnumerable<JsonValidationError> HaveValidIndexes(List<DeadlineIndex> jsonModel, Dictionary<string, List<Step>> protocols)
    {
        foreach (var deadlineIndex in jsonModel)
        {
            if (deadlineIndex.End < deadlineIndex.Start)
                yield return new InvalidStructure($"{deadlineIndex.End} < {deadlineIndex.Start}",
                    "End index of critical section cannot be smaller than start index");

            if (deadlineIndex.Start < 0)
                yield return new InvalidStructure($"{nameof(DeadlineIndex.Start)}<0",
                    "The start index of a critical section cannot be negative");

            if (deadlineIndex.Time < 0)
                yield return new InvalidStructure($"{deadlineIndex.Time}<0",
                    "The deadline time cannot be smaller than 0");
        }
    }

    public static IEnumerable<JsonValidationError> NotHaveUndefinedParts(List<DeadlineIndex> jsonModel, Dictionary<string, List<Step>> protocols)
    {
        return jsonModel
            .Where(e=>!protocols.ContainsKey(e.Product))
            .Select(e => new UndeclaredStructure(e.Product, nameof(CatProblemModel.Deadline), nameof(CatProblemModel.Protocol)));
    }
}