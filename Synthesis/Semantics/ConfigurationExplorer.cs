using ModelChecker.Domain;
using ModelChecker.Search;

namespace ModelChecker.Semantics;

internal sealed class ConfigurationExplorer : IConfigurationExplorer
{
    private readonly IActionExecutor _actionExecutor;
    private readonly PossibleDelayComputer _delayComputer;

    internal ConfigurationExplorer(PossibleDelayComputer delayComputer, IActionExecutor executor)
    {
        _delayComputer = delayComputer;
        _actionExecutor = executor;
    }

    /// <inheritdoc />
    public IEnumerable<ReachableConfiguration> GenerateConfigurations(Configuration configuration)
    {
        var delays = _delayComputer.ComputePossibleDelays(configuration);
        var possibleActions = ActionGenerator.GetPossibleActions(configuration);

        foreach (var action in possibleActions)
        {
            yield return ReachableConfiguration.FromAction(action, _actionExecutor.Execute(action, configuration));
        }
        foreach (var delay in delays)
        {
            yield return ReachableConfiguration.FromAction(delay, _actionExecutor.Execute(delay, configuration));
        }

    }
}