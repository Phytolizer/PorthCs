using System.Diagnostics;

namespace PorthCs;

internal static class Simulator
{
    public static void Simulate(IList<Op> program)
    {
        Debug.Assert((int)OpCode.Count == 8, "OpCodes are not exhaustively handled in Simulator.Simulate.");
        var stack = new Stack<object>();
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
                    stack.Push(a == b ? 1 : 0);
                    ++ip;
                    break;
                }
                case OpCode.Dump:
                {
                    var a = stack.Pop();
                    Console.WriteLine(a);
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
                    break;
                case OpCode.Else:
                {
                    var elseOp = (IntegerOp)op;
                    ip = (int)elseOp.Operand;
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