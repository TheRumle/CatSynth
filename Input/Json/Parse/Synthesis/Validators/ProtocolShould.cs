using Common.Json.Validation;
using Scheduler.Input.Json.Models;
using Scheduler.Input.Json.Parse.Synthesis.Errors;

namespace Scheduler.Input.Json.Parse.Synthesis.Validators;

public static class ProtocolShould
{
    public static IEnumerable<JsonValidationError> HaveMachinesDeclared(Dictionary<string, List<Step>> protocol,
        IEnumerable<MachineModel> machineModels)
    {
        return protocol.SelectMany(kvp =>
        {
            return kvp
                .Value
                .Where(e => !machineModels.Select(e => e.Name).Contains(e.Machine))
                .Select(e => new UndeclaredStructure(e.Machine, kvp.Key, nameof(CatProblemModel.Machines)));
        });
    }

    public static IEnumerable<JsonValidationError> HaveMeaningfulConstants(Dictionary<string, List<Step>> protocol)
    {
        return protocol
            .Select(kvp => (product: kvp.Key, steps: kvp.Value))
            .SelectMany(kvp =>
            {
                return kvp.steps
                    .Where(e => e.MaxTime < e.MinTime)
                    .Select(e => new InvalidStructure($"{nameof(CatProblemModel.Protocol)}, {kvp.product}", "Min > Max"));
            });
    }
}