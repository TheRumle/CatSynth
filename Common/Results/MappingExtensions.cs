using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Common.Results;

public static class MappingExtensions
{
    [Pure]
    public static Result<TTarget> MapTo<TValue, TTarget>(this Result<TValue> result, Func<TValue, TTarget> map)
    {
        if (result.IsFailure) return Result.Failure<TTarget>(result.Errors);
        var a = new List<int>();
        return Result.Success(map(result.Value));
    }

    public static Result<TTarget> Map<TValue, TTarget>(this Task<Result<TValue>> task, Func<TValue, TTarget> onSuccess)
    {
        task.Wait();
        if (!task.IsCompletedSuccessfully)
            return Result.Failure<TTarget>(new Error[] { Error.TaskError(task.Exception!.Flatten()) });
        return task.Result.IsSuccess 
            ? Result.Success(onSuccess.Invoke(task.Result.Value)) 
            : Result.Failure<TTarget>(task.Result.Errors);
    }

    public static Result<TOut> Select<T,TOut>(this Task<T> task, Func<T, TOut> map, Error onError)
    {
        task.Wait();
        if (!task.IsCompletedSuccessfully)
            return Result.Failure<TOut>(onError);

        return Result.Success(map.Invoke(task.Result));
    }

    public static Result<TOut> Select<T,TOut>(this Task<T> task, Func<T, TOut> map)
    {
        task.Wait();
        if (!task.IsCompletedSuccessfully)
            return Result.Failure<TOut>([Error.TaskError(task.Exception)]);

        return Result.Success(map.Invoke(task.Result));
    }
    
    public static Result Select<T>(this Task<T> task, Func<T, Result> map)
    {
        task.Wait();
        if (!task.IsCompletedSuccessfully)
            return Result.Failure([Error.TaskError(task.Exception)]);

        return Result.Success(map.Invoke(task.Result));
    }

    
    public static Result<T> Collapse<T>(this Result<Result<T>> result)
    {
        if (result.IsFailure)
        {
            return Result.Failure<T>(result.Errors);
        }

        if (result.Value.IsFailure)
        {
            return Result.Failure<T>(result.Value.Errors);
        }

        return result.Value;
    }

    public static Result<IEnumerable<T>> Aggregate<T>(this IEnumerable<Result<T>> enumerable)
    {
        var results = enumerable as Result<T>[] ?? enumerable.ToArray();
        if (results.Length > 1 && results.First().IsSuccess)
        {
            return Result.Success<IEnumerable<T>>([results.First().Value]);
        }

        if (!results.Any(e => e.IsFailure)) 
            return Result.Success(results.Select(e => e.Value));
        
        var failure = results.Aggregate(Result.Success(), (f, s) => f.Collapse(s));
        return Result.Failure<IEnumerable<T>>(failure.Errors);
    }

    
    public static Result Aggregate(this IEnumerable<Result> enumerable)
    {
        var results = enumerable as Result[] ?? enumerable.ToArray();
        if (results.All(e => e.IsSuccess))
            return Result.Success();
        
        var failure = results.Aggregate(Result.Success(), (f, s) => f.Collapse(s));
        return Result.Failure(failure.Errors);
    }
}