
namespace Common.Json.Validation;

public interface IValidator<T, TContext>
{
    public IEnumerable<JsonValidationError> Validate(T values, TContext context);
}

public interface IValidator<T>
{
    public IEnumerable<JsonValidationError> Validate(T values);
    public Task<IEnumerable<JsonValidationError>> ValidateAsync(T values);
}