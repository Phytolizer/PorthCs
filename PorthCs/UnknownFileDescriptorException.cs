namespace PorthCs;

internal class UnknownFileDescriptorException : Exception
{
    public UnknownFileDescriptorException(ulong fd)
    {
        Message = $"Unknown file descriptor {fd}.";
    }

    public override string Message { get; }
}
