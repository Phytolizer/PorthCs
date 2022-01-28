﻿using System.Diagnostics;

namespace PorthCs;

internal static class Parser
{
    public static Op ParseTokenAsOp(Token tok)
    {
        Debug.Assert((int)OpCode.Count == 21, "OpCodes are not exhaustively handled in Parser.ParseTokenAsOp.");
        switch (tok.Word)
        {
            case "+":
                return Ops.Plus(tok);
            case "-":
                return Ops.Minus(tok);
            case "dump":
                return Ops.Dump(tok);
            case "=":
                return Ops.Equal(tok);
            case ">":
                return Ops.Gt(tok);
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
            case "mem":
                return Ops.Mem(tok);
            case ",":
                return Ops.Load(tok);
            case ".":
                return Ops.Store(tok);
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
