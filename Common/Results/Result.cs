using System.Diagnostics.Contracts;

namespace Common.Results;

public class Result
{
    /// <inheritdoc />
    public override string ToString()
    {
        return this.IsFailure
            ? $"Failed: {string.Join(",", Errors.Select(e => e.ToString()))}"
            : "Success";
    }

    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Errors = [error];
    }

    protected Result(bool isSuccess, Error[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    private Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
        Errors = [];
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error[] Errors { get; }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result<TValue> Success<TValue>(TValue value)
    {
        return new Result<TValue>(value);
    }

    [Pure]
    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    [Pure]
    public static Result Failure(Error[] errors)
    {
        return new Result(false, errors);
    }

    [Pure]
    public static Result<T> Failure<T>(IEnumerable<Error> errors)
    {
        return new Result<T>(default, false, errors.ToArray());
    }

    [Pure]
    public static Result<T> Failure<T>()
    {
        return new Result<T>(default, false, []);
    }

    [Pure]
    public static Result<T> Failure<T>(Error errors)
    {
        return new Result<T>(default, false, errors);
    }
}

public class Result<T> : Result
{
    internal Result(T value, bool isSuccess, Error error)
        : this(value, isSuccess, [error])
    {
    }

    internal Result(T value) : this(value, true, [])
    {
    }

    internal Result(T value, bool isSuccess, Error[] errors) : base(isSuccess, errors)
    {
        Value = value;
    }

    public T Value { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsFailure
            ? $"Failed: {string.Join(",", Errors.Select(e => e.ToString()))}"
            : $"{Value!.ToString()}";
    }
}