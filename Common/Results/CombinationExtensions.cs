namespace Common.Results;

public static class CombinationExtensions
{
    public static Result Collapse(this Result result, Result other)
    {
        if (result.IsFailure || other.IsFailure)
        {
            List<Error> errors = new List<Error>();
            errors.AddRange(result.Errors);
            errors.AddRange(other.Errors);
            return Result.Failure(errors.ToArray());
        } 
        return result;
    }
    
    public static Result Collapse(this Result result, params Result[] others)
    {
        return others.Aggregate(result, (first, second) => first.Collapse(second));
    }
    
    
    
    public static Result<T> Collapse<T>(this Result<T> result, Result<T> other)
    {
        if (result.IsFailure || other.IsFailure)
        {
            List<Error> errors = new List<Error>();
            errors.AddRange(result.Errors);
            errors.AddRange(other.Errors);
            return Result.Failure<T>(errors.ToArray());
        } 
        return result;
    }
    
    public static Result<T> Collapse<T>(this Result<T> result, IEnumerable<Result<T>> others)
    {
        return others.Aggregate(result, (first, second) => first.Collapse(second));
    }


    public static Result<(TFirst first, TSecond second)> Combine<TFirst, TSecond>(this Result<TFirst> result, Result<TSecond> other)
    {
        if (result.IsFailure || other.IsFailure)
        {
            List<Error> errors = new List<Error>();
            errors.AddRange(result.Errors);
            errors.AddRange(other.Errors);
            return Result.Failure<(TFirst first, TSecond second)>(errors.ToArray());
        }
        return Result.Success((result.Value, other.Value));
    }
    
    public static Result<(TFirst first, TSecond second)> Combine<TFirst, TSecond>(this Result<TFirst> result, TSecond other)
    {
        if (result.IsFailure)
        {
            return Result.Failure<(TFirst first, TSecond second)>(result.Errors);
        }
        return Result.Success((result.Value, other));
    }
  
  
    
}