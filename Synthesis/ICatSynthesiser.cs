using Common.Results;

namespace ModelChecker;

public interface ICatSynthesiser : IDisposable
{
    string Name { get; }
    int NumberOfConfigurationsExplored { get; }

    /// <summary>
    ///     Runs the model checker until a either a solution is found or the timeout is reached.
    /// </summary>
    /// <param name="timeOut">
    ///     The time before the checker will stop exploring more configurations, resulting in a Failed
    ///     result.
    /// </param>
    /// <returns>A result containing the schedule if found, failure otherwise</returns>
    Result<Schedule> Execute(TimeSpan timeOut);

    /// <summary>
    ///     Runs the model checker until a either a solution is found or the timeout is reached.
    /// </summary>
    /// <param name="timeOut">
    ///     The time before the checker will stop exploring more configurations, resulting in a Failed
    ///     result.
    /// </param>
    /// <returns>A result containing the schedule if found, failure otherwise</returns>
    Task<Result<Schedule>> ExecuteAsync(TimeSpan timeOut);
}