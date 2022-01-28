using System.Diagnostics;

namespace PorthCs;

internal static class SemanticAnalyzer
{
    public static IEnumerable<Op> CrossReferenceBlocks(List<Op> program)
    {
        var stack = new Stack<int>();
        for (var ip = 0; ip < program.Count; ++ip)
        {
            var op = program[ip];
            switch (op.Code)
            {
                case OpCode.If:
                case OpCode.While:
                    stack.Push(ip);
                    break;
                case OpCode.Else:
                {
                    var ifIp = stack.Pop();
                    if (program[ifIp].Code != OpCode.If)
                    {
                        throw new SemanticError(op, "'else' can only be used in 'if'-blocks.");
                    }

                    program[ifIp] = new IntegerOp(program[ifIp], (ulong)ip + 1);
                    stack.Push(ip);
                    break;
                }
                case OpCode.End:
                {
                    var blockIp = stack.Pop();
                    switch (program[blockIp].Code)
                    {
                        case OpCode.If or OpCode.Else:
                            program[blockIp] = new IntegerOp(program[blockIp], (ulong)ip);
                            program[ip] = new IntegerOp(program[ip], (ulong)ip + 1);
                            break;
                        case OpCode.Do:
                        {
                            var doOp = (IntegerOp)program[blockIp];
                            // This 'end' shall point to the 'while' for simulation.
                            program[ip] = new IntegerOp(program[ip], doOp.Operand);
                            var whileIp = (int)doOp.Operand;
                            // The 'while' shall point to after this 'end' for simulation.
                            program[whileIp] = new IntegerOp(program[whileIp], (ulong)ip + 1);
                            break;
                        }
                        default:
                            throw new SemanticError(op, "'end' does not close an 'if'/'else'/'while' block.");
                    }

                    break;
                }
                case OpCode.Do:
                {
                    var blockIp = stack.Pop();
                    if (program[blockIp].Code != OpCode.While)
                    {
                        throw new SemanticError(op, "'do' does not follow 'while'.");
                    }

                    // This 'do' shall point to the 'while' for simulation and compilation.
                    // For simulation, it is used to access the 'while' operand (which points after the loop body).
                    // For compilation, it is used to break the Rust loop label.
                    program[ip] = new IntegerOp(program[ip], (ulong)blockIp);
                    stack.Push(ip);
                    break;
                }
                case OpCode.Push:
                case OpCode.Plus:
                case OpCode.Minus:
                case OpCode.Equal:
                case OpCode.Lt:
                case OpCode.Gt:
                case OpCode.Dump:
                case OpCode.Dup:
                case OpCode.Dup2:
                case OpCode.Swap:
                case OpCode.Drop:
                case OpCode.Mem:
                case OpCode.Load:
                case OpCode.Store:
                case OpCode.Syscall1:
                case OpCode.Syscall2:
                case OpCode.Syscall3:
                case OpCode.Syscall4:
                case OpCode.Syscall5:
                case OpCode.Syscall6:
                    break;
                case OpCode.Count:
                    Debug.Fail("This is unreachable.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }

        return program;
    }
}

internal class SemanticError : Exception
{
    private readonly Op _op;
    private readonly string _message;

    public SemanticError(Op op, string message)
    {
        _op = op;
        _message = message;
    }

    public override string Message => $"{_op.FilePath}:{_op.LineNumber}:{_op.ColumnNumber}: {_message}";
}
