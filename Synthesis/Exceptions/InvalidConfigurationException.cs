namespace ModelChecker.Exceptions;

public class InvalidConfigurationException(string message) : Exception(message)
{
    private readonly string _message = message;

    /// <inheritdoc />
    public override string ToString()
    {
        return this._message;
    }
}