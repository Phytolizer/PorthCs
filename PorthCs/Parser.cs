namespace PorthCs;

internal static class Parser
{
    public static Op ParseTokenAsOp(Token tok)
    {
        switch (tok.Word)
        {
            case "+":
                return Ops.Plus(tok);
            case "-":
                return Ops.Minus(tok);
            case "mod":
                return Ops.Mod(tok);
            case "print":
                return Ops.Print(tok);
            case "=":
                return Ops.Eq(tok);
            case "!=":
                return Ops.Ne(tok);
            case "<":
                return Ops.Lt(tok);
            case ">":
                return Ops.Gt(tok);
            case "<=":
                return Ops.Le(tok);
            case ">=":
                return Ops.Ge(tok);
            case "shr":
                return Ops.Shr(tok);
            case "shl":
                return Ops.Shl(tok);
            case "bor":
                return Ops.Bor(tok);
            case "band":
                return Ops.Band(tok);
            case "if":
                return Ops.If(tok);
            case "end":
                return Ops.End(tok);
            case "else":
                return Ops.Else(tok);
            case "while":
                return Ops.While(tok);
            case "do":
                return Ops.Do(tok);
            case "dup":
                return Ops.Dup(tok);
            case "2dup":
                return Ops.Dup2(tok);
            case "swap":
                return Ops.Swap(tok);
            case "drop":
                return Ops.Drop(tok);
            case "over":
                return Ops.Over(tok);
            case "mem":
                return Ops.Mem(tok);
            case ",":
                return Ops.Load(tok);
            case ".":
                return Ops.Store(tok);
            case "syscall0":
                return Ops.Syscall0(tok);
            case "syscall1":
                return Ops.Syscall1(tok);
            case "syscall2":
                return Ops.Syscall2(tok);
            case "syscall3":
                return Ops.Syscall3(tok);
            case "syscall4":
                return Ops.Syscall4(tok);
            case "syscall5":
                return Ops.Syscall5(tok);
            case "syscall6":
                return Ops.Syscall6(tok);
            default:
                if (ulong.TryParse(tok.Word, out var value))
                {
                    return Ops.Push(tok, value);
                }

                throw new ParserError(tok, "Unknown token");
        }
    }
}
