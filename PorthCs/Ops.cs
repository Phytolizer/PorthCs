namespace PorthCs;

internal static class Ops
{
    public static Op Push(Token t, ulong x)
    {
        return new IntegerOp(OpCode.Push, t, x);
    }

    public static Op Plus(Token t)
    {
        return new Op(OpCode.Plus, t);
    }

    public static Op Minus(Token t)
    {
        return new Op(OpCode.Minus, t);
    }

    public static Op Mod(Token t)
    {
        return new Op(OpCode.Mod, t);
    }

    public static Op Dump(Token t)
    {
        return new Op(OpCode.Dump, t);
    }

    public static Op Eq(Token t)
    {
        return new Op(OpCode.Eq, t);
    }

    public static Op Lt(Token t)
    {
        return new Op(OpCode.Lt, t);
    }

    public static Op Gt(Token t)
    {
        return new Op(OpCode.Gt, t);
    }

    public static Op Shr(Token t)
    {
        return new Op(OpCode.Shr, t);
    }

    public static Op Shl(Token t)
    {
        return new Op(OpCode.Shl, t);
    }

    public static Op Bor(Token t)
    {
        return new Op(OpCode.Bor, t);
    }

    public static Op Band(Token t)
    {
        return new Op(OpCode.Band, t);
    }

    public static Op If(Token t)
    {
        return new Op(OpCode.If, t);
    }

    public static Op End(Token t)
    {
        return new Op(OpCode.End, t);
    }

    public static Op Else(Token t)
    {
        return new Op(OpCode.Else, t);
    }

    public static Op While(Token t)
    {
        return new Op(OpCode.While, t);
    }

    public static Op Do(Token t)
    {
        return new Op(OpCode.Do, t);
    }

    public static Op Dup(Token t)
    {
        return new Op(OpCode.Dup, t);
    }

    public static Op Dup2(Token t)
    {
        return new Op(OpCode.Dup2, t);
    }

    public static Op Swap(Token t)
    {
        return new Op(OpCode.Swap, t);
    }

    public static Op Drop(Token t)
    {
        return new Op(OpCode.Drop, t);
    }

    public static Op Over(Token t)
    {
        return new Op(OpCode.Over, t);
    }

    public static Op Mem(Token t)
    {
        return new Op(OpCode.Mem, t);
    }

    public static Op Load(Token t)
    {
        return new Op(OpCode.Load, t);
    }

    public static Op Store(Token t)
    {
        return new Op(OpCode.Store, t);
    }

    public static Op Syscall1(Token t)
    {
        return new Op(OpCode.Syscall1, t);
    }

    public static Op Syscall3(Token t)
    {
        return new Op(OpCode.Syscall3, t);
    }

    public static Op Syscall2(Token t)
    {
        return new Op(OpCode.Syscall2, t);
    }

    public static Op Syscall4(Token t)
    {
        return new Op(OpCode.Syscall4, t);
    }

    public static Op Syscall5(Token t)
    {
        return new Op(OpCode.Syscall5, t);
    }

    public static Op Syscall6(Token t)
    {
        return new Op(OpCode.Syscall6, t);
    }
}
