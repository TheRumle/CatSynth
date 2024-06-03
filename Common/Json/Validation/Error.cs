using Common.Results;

namespace Common.Json.Validation;

public abstract class JsonValidationError 
{
    public abstract string ErrorCategory { get; }
    public abstract override string ToString();
}

public static class ToError
{
    public static IEnumerable<Error> ToErrors(this IEnumerable<JsonValidationError> errors)
    {
        return errors.GroupBy(e => e.ErrorCategory)
            .Select(e=>new Error(
                errorName: e.Key,
                description: e.Aggregate("\t",(prev,validationErr) => prev + validationErr+"\n" )
            ));
    }
    
    public static Result<T> ToFailedResult<T>(this IEnumerable<JsonValidationError> errors)
    {
        return Result.Failure<T>(ToErrors(errors));
    }

}