namespace PorthCs;

internal static class Ops
{
    public static Op Push(Token t, ulong x)
    {
        return new IntegerOp(OpCode.Push, t.FilePath, t.LineNumber, t.ColumnNumber, x);
    }

    public static Op Plus(Token t)
    {
        return new Op(OpCode.Plus, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Minus(Token t)
    {
        return new Op(OpCode.Minus, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Dump(Token t)
    {
        return new Op(OpCode.Dump, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Equal(Token t)
    {
        return new Op(OpCode.Equal, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op If(Token t)
    {
        return new Op(OpCode.If, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op End(Token t)
    {
        return new Op(OpCode.End, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Else(Token t)
    {
        return new Op(OpCode.Else, t.FilePath, t.LineNumber, t.ColumnNumber);
    }
}