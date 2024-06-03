namespace Common;

public class Error(string errorName, string description)
{
    public static readonly Error NullError = new(nameof(NullError), "A value could not be obtained");
    public static readonly Error None = new(nameof(None), "The result could be achieved without errors");

    public static Error FileReadError(FileInfo fileInfo, Exception e) =>
        new(nameof(FileReadError), $"Could not read file {fileInfo.FullName}. It threw the following exception: {e}");
    public string ErrorName { get; init; } = errorName;
    public string Description { get; init; } = description;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"ERROR: {ErrorName}: {description}";
    }

    public static Error TaskError(Exception e) => new(nameof(TaskError), $"Task failed with error {e}");
}

