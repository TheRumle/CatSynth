using ModelChecker.Domain;

namespace ModelChecker.Search;

internal interface IConfigurationExplorer
{
    public IEnumerable<ReachableConfiguration> GenerateConfigurations(Configuration configuration);
}