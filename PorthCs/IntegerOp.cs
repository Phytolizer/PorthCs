namespace PorthCs;

internal class IntegerOp : Op
{
    public ulong Operand { get; }

    public IntegerOp(OpCode code, ulong operand) : base(code)
    {
        Operand = operand;
    }
}