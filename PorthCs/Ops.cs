namespace PorthCs;

internal static class Ops
{
    public static Op Push(ulong x)
    {
        return new IntegerOp(OpCode.Push, x);
    }

    public static Op Plus()
    {
        return new Op(OpCode.Plus);
    }

    public static Op Minus()
    {
        return new Op(OpCode.Minus);
    }

    public static Op Dump()
    {
        return new Op(OpCode.Dump);
    }

    public static Op Equal()
    {
        return new Op(OpCode.Equal);
    }
}