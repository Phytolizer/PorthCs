namespace PorthCs;

internal class SubcommandException : Exception
{
    private readonly string[] _command;

    public SubcommandException(string[] command)
    {
        _command = command;
    }

    public override string Message => $"Failed to start '{string.Join(' ', _command)}'";
}