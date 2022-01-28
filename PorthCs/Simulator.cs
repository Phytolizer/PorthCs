using System.Diagnostics;

namespace PorthCs;

internal static class Simulator
{
    public static void Simulate(IList<Op> program, TextWriter writer, bool debugMode)
    {
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
                case OpCode.Lt:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push((ulong)(a < b ? 1 : 0));
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
                case OpCode.Dup2:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(a);
                    stack.Push(b);
                    stack.Push(a);
                    stack.Push(b);
                    ++ip;
                    break;
                }
                case OpCode.Swap:
                {
                    var b = (ulong)stack.Pop();
                    var a = (ulong)stack.Pop();
                    stack.Push(b);
                    stack.Push(a);
                    ++ip;
                    break;
                }
                case OpCode.Drop:
                    stack.Pop();
                    ++ip;
                    break;
                case OpCode.Mem:
                    stack.Push((ulong)0);
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
                case OpCode.Syscall1:
                {
                    throw new NotImplementedException("Syscall1");
                }
                case OpCode.Syscall2:
                {
                    throw new NotImplementedException("Syscall2");
                }
                case OpCode.Syscall3:
                {
                    var syscall = (ulong)stack.Pop();
                    var arg1 = (ulong)stack.Pop();
                    var arg2 = (ulong)stack.Pop();
                    var arg3 = (ulong)stack.Pop();
                    switch (syscall)
                    {
                        case 1:
                        {
                            switch (arg1)
                            {
                                case 1:
                                    writer.Write(mem[(int)arg2..(int)(arg2 + arg3)]);
                                    break;
                                case 2:
                                    Console.Error.WriteLine(mem[(int)arg2..(int)(arg2 + arg3)]);
                                    break;
                                default:
                                    throw new UnknownFileDescriptorException(arg1);
                            }

                            break;
                        }
                        default:
                            throw new UnknownSyscallException(syscall);
                    }

                    ++ip;
                    break;
                }
                case OpCode.Syscall4:
                {
                    throw new NotImplementedException("Syscall4");
                }
                case OpCode.Syscall5:
                {
                    throw new NotImplementedException("Syscall5");
                }
                case OpCode.Syscall6:
                {
                    throw new NotImplementedException("Syscall6");
                }
                case OpCode.Count:
                    throw new InvalidOperationException("unreachable");
                default:
                    throw new ArgumentOutOfRangeException(nameof(program));
            }
        }

        if (!debugMode)
        {
            return;
        }

        Console.WriteLine("[INFO] Memory dump");
        Console.WriteLine(mem[..20]);
    }
}
