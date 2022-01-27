namespace PorthCs;

internal class UnknownSyscallException : Exception
{
    public UnknownSyscallException(ulong syscall)
    {
        Message = $"Unknown syscall {syscall}.";
    }

    public override string Message { get; }
}
