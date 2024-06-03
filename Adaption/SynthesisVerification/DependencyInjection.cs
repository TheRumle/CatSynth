using Microsoft.Extensions.DependencyInjection;

namespace CatConversion.SynthesisVerification;

public static class DependencyInjection
{
    public static IServiceCollection AddScheduleToCatAdaption(this IServiceCollection collection)
    {
        return collection
            .AddSingleton<ScheduleCatVerifier>();
    } 
}