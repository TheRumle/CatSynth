using Microsoft.Extensions.DependencyInjection;
using ModelChecker.Factory;

namespace ModelChecker;

public static class AddModelChecking
{
    public static IServiceCollection AddSynthesisServices(this IServiceCollection collection)
    {
        return collection
            .AddSingleton(new MachineFactory())
            .AddSingleton(new SynthesiserFactory())
            .AddSingleton(new SynthesizerCollection())
            .AddSingleton<HeuristicCollection>();
    }
}