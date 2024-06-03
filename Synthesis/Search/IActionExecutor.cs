using ModelChecker.Domain;
using ModelChecker.Domain.Actions;

namespace ModelChecker.Search;

public interface IActionExecutor
{
    Configuration Execute(SystemAction action, Configuration configuration);
}