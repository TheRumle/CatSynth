using System.Linq.Expressions;
using Xunit.Abstractions;

namespace Scheduler.ModelChecker.IntegrationTest;

public abstract class IntegrationTest<TResult>(ITestOutputHelper output) where TResult : class
{
    protected ITestOutputHelper Output = output;
    protected TResult? Result = null;
    
    /// <inheritdoc />
    public void Dispose()
    {
        if (Result is not null)
        {
            Output.WriteLine(Result.ToString());
            Output.WriteLine($"\n\n");
            
        }
    }

    
}