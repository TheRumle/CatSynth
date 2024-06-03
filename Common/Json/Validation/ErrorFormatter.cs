using System.Text;

namespace Common.Json.Validation;

public class ErrorFormatter
{

    public ErrorFormatter(IEnumerable<JsonValidationError> errors)
    {
        Errors = errors;
    }

    public IEnumerable<JsonValidationError> Errors { get; init; }

    public string ToErrorString()
    {
        var bob = new StringBuilder($"Found errors when parsing json:\n");
        foreach (var errGroup in  Errors.GroupBy(e => e.ErrorCategory))
        {
            bob.AppendLine($"'{errGroup.Key} errors:'").Append('\n');
            foreach (var exception in errGroup) bob.Append('\t').Append(exception).Append("\n\n");
        }

        return bob.ToString();
    }

    public IEnumerable<Error> ToDomainErrors()
    {
        return Errors.GroupBy(e => e.ErrorCategory)
            .Select(e=>new Error(
                errorName: e.Key,
                description: e.Aggregate("\t",(prev,validationErr) => prev + validationErr+"\n" )
                ));
    }
}