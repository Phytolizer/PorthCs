namespace PorthCs;

internal class IntegerOp : Op
{
    public ulong Operand { get; }

    public IntegerOp(OpCode code, Token token, ulong operand) : base(code, token)
    {
        Operand = operand;
    }

    public IntegerOp(Op op, ulong operand) : base(op.Code, op.Token)
    {
        Operand = operand;
    }

    public override string ToString()
    {
        return $"{Code}({Operand})";
    }
}
