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

    public static Op Gt(Token t)
    {
        return new Op(OpCode.Gt, t.FilePath, t.LineNumber, t.ColumnNumber);
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

    public static Op While(Token t)
    {
        return new Op(OpCode.While, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Do(Token t)
    {
        return new Op(OpCode.Do, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Dup(Token t)
    {
        return new Op(OpCode.Dup, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Mem(Token t)
    {
        return new Op(OpCode.Mem, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Load(Token t)
    {
        return new Op(OpCode.Load, t.FilePath, t.LineNumber, t.ColumnNumber);
    }

    public static Op Store(Token t)
    {
        return new Op(OpCode.Store, t.FilePath, t.LineNumber, t.ColumnNumber);
    }
}
