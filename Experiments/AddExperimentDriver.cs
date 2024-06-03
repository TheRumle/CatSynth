using Experiments.ExperimentDriver;
using Experiments.ResultWriter;
using Microsoft.Extensions.DependencyInjection;

namespace Experiments;

public static class AddExperimentDriver
{
    public static IServiceCollection AddExperimentRunnerServices(this IServiceCollection collection)
    {
        return collection
            .AddScoped<ExperimentRunner>()
            .AddScoped<ExperimentResultWriter>();
    }
}