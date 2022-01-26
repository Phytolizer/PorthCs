﻿namespace PorthCs;

internal class IntegerOp : Op
{
    public ulong Operand { get; }

    public IntegerOp(OpCode code, string filePath, int lineNumber, int columnNumber, ulong operand) : base(code, filePath, lineNumber, columnNumber)
    {
        Operand = operand;
    }

    public IntegerOp(Op op, ulong operand) : base(op.Code, op.FilePath, op.LineNumber, op.ColumnNumber)
    {
        Operand = operand;
    }

    public override string ToString() => $"{Code}({Operand})";
}