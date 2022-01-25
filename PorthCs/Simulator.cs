using System.Diagnostics;

namespace PorthCs;

internal static class Simulator
{
    public static void Simulate(IList<Op> program)
    {
        Debug.Assert((int)OpCode.Count == 7, "OpCodes are not exhaustively handled in Simulator.Simulate.");
        var stack = new Stack<object>();
        for (var i = 0; i < program.Count;)
        {
            var op = program[i];
            switch (op.Code)
            {
                case OpCode.Push:
                {
                    var pushOp = (IntegerOp)op;
                    stack.Push(pushOp.Operand);
                    ++i;
                    break;
                }
                case OpCode.Plus:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a + b);
                    ++i;
                    break;
                }
                case OpCode.Minus:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a - b);
                    ++i;
                    break;
                }
                case OpCode.Equal:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a == b ? 1 : 0);
                    ++i;
                    break;
                }
                case OpCode.Dump:
                {
                    var a = stack.Pop();
                    Console.WriteLine(a);
                    ++i;
                    break;
                }
                case OpCode.If:
                {
                    var ifOp = (IntegerOp)op;
                    var cond = (ulong)stack.Pop();
                    if (cond == 0)
                    {
                        i = (int)ifOp.Operand;
                    }
                    else
                    {
                        ++i;
                    }

                    break;
                }
                case OpCode.End:
                    break;
                case OpCode.Count:
                    Debug.Fail("This is unreachable.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }
    }
}