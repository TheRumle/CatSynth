namespace Common.Results;

public static class ErrorToException
{
    public static Exception ToException(this IEnumerable<Error> errors)
    {
        
        return new Exception(errors.Aggregate("", (s, e) => $"{s}\n{e.ErrorName}: {e.Description}"));
    }


}