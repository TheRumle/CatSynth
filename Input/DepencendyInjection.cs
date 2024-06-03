using Common.Json;
using Common.Json.Validation;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.Input.FileRead;
using Scheduler.Input.Json;
using Scheduler.Input.Json.Models;
using Scheduler.Input.Json.Parse.Synthesis;
using Scheduler.Input.Json.Parse.Synthesis.Validators;

namespace Scheduler.Input;

public static class DependencyInjection
{

    public static IServiceCollection AddJsonInputServices(this IServiceCollection collection)
    {
        return collection
            .AddScoped<FileNameParser>()
            .AddScoped<CatSystemValidator>()
            .AddScoped<CatProblemValidator>()
            .AddScoped<IValidator<InputSequenceModel>, InputSequenceValidator>()
            .AddScoped<IValidator<CatProblemModel>, CatProblemValidator>()
            .AddScoped<IValidator<CatSystemModel>, CatSystemValidator>()
            .AddScoped<IJsonParser<CatSystemModel>, CatSystemModelParser>()
            .AddScoped<IJsonParser<InputSequenceModel>, InputSequenceParser>()
            .AddScoped<JsonSynthesisProblemLoader>()
            .AddScoped<SynthesisProblemMapper>();
    }
}