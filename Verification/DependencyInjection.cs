using Cat.Verify.Verification;
using Microsoft.Extensions.DependencyInjection;

namespace Cat.Verify;

public static class DependencyInjection
{
    public static IServiceCollection AddCatVerificationServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<ExecutionVerifierFactory>();
    }
    
}