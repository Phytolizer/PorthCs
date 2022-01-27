﻿using System.Diagnostics;

namespace PorthCs;

internal static class Simulator
{
    public static void Simulate(IList<Op> program, TextWriter writer)
    {
        Debug.Assert((int)OpCode.Count == 15, "OpCodes are not exhaustively handled in Simulator.Simulate.");
        var stack = new Stack<object>();
        var mem = new char[Memory.Capacity];
        for (var ip = 0; ip < program.Count;)
        {
            var op = program[ip];
            switch (op.Code)
            {
                case OpCode.Push:
                {
                    var pushOp = (IntegerOp)op;
                    stack.Push(pushOp.Operand);
                    ++ip;
                    break;
                }
                case OpCode.Plus:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a + b);
                    ++ip;
                    break;
                }
                case OpCode.Minus:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a - b);
                    ++ip;
                    break;
                }
                case OpCode.Equal:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push((ulong)(a == b ? 1 : 0));
                    ++ip;
                    break;
                }
                case OpCode.Gt:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push((ulong)(a > b ? 1 : 0));
                    ++ip;
                    break;
                }
                case OpCode.Dump:
                {
                    var a = stack.Pop();
                    writer.WriteLine(a);
                    ++ip;
                    break;
                }
                case OpCode.If:
                {
                    var ifOp = (IntegerOp)op;
                    var cond = (ulong)stack.Pop();
                    if (cond == 0)
                    {
                        ip = (int)ifOp.Operand;
                    }
                    else
                    {
                        ++ip;
                    }

                    break;
                }
                case OpCode.End:
                {
                    var endOp = (IntegerOp)op;
                    ip = (int)endOp.Operand;
                    break;
                }
                case OpCode.Else:
                {
                    var elseOp = (IntegerOp)op;
                    ip = (int)elseOp.Operand;
                    break;
                }
                case OpCode.While:
                    ++ip;
                    break;
                case OpCode.Do:
                {
                    var a = (ulong)stack.Pop();
                    if (a == 0)
                    {
                        var doOp = (IntegerOp)op;
                        var whileOp = (IntegerOp)program[(int)doOp.Operand];
                        ip = (int)whileOp.Operand;
                    }
                    else
                    {
                        ++ip;
                    }

                    break;
                }
                case OpCode.Dup:
                {
                    var a = (ulong)stack.Pop();
                    stack.Push(a);
                    stack.Push(a);
                    ++ip;
                    break;
                }
                case OpCode.Mem:
                    stack.Push(0);
                    ++ip;
                    break;
                case OpCode.Load:
                {
                    var a = (ulong)stack.Pop();
                    stack.Push((ulong)mem[a]);
                    ++ip;
                    break;
                }
                case OpCode.Store:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    mem[a] = (char)b;
                    ++ip;
                    break;
                }
                case OpCode.Count:
                    Debug.Fail("This is unreachable.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }
    }
}
