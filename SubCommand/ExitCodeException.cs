namespace SubCommand;

public class ExitCodeException : Exception
{
    public ExitCodeException(string[] args)
    {
        Message = $"{string.Join(' ', args)} returned non-zero exit code.";
    }

    public override string Message { get; }
}