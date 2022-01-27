namespace SubCommand;

public class StartException : Exception
{
    public StartException(string[] args)
    {
        Message = $"{string.Join(' ', args)} failed to start.";
    }

    public override string Message { get; }
}