namespace Common.Json.Validation;

public abstract class Validator<T> : IValidator<T> 
{

        public IEnumerable<JsonValidationError> Validate(T values)
        {
            var validationTasks = ValidationTasksFor(values);
            return Task.WhenAll(validationTasks).Result.SelectMany(errs=>errs);
        }

        /// <inheritdoc />
        public Task<IEnumerable<JsonValidationError>> ValidateAsync(T values)
        {
            return Task.Run(()=>Validate(values));
        }

        public abstract Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(T jsonModel);

        protected Task<IEnumerable<JsonValidationError>>[] BeginValidationsOver(T input, IEnumerable<Func<T, IEnumerable<JsonValidationError>>> validationActions)
        {
            return validationActions.Select(e=> Task.Run(()=>e.Invoke(input))).ToArray();
        }
}

public abstract class JsonValidator<T, TT> : IValidator<T, TT>
{
    public IEnumerable<JsonValidationError> Validate(T values, TT context)
    {
        var validationTasks = ValidationTasksFor(values, context);
        return Task.WhenAll(validationTasks).Result.SelectMany(e=>e);
    }

    public abstract Task<IEnumerable<JsonValidationError>>[] ValidationTasksFor(T inputs, TT context);
    

}
