using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public enum Operators
    {
        //If changed, change BinaryOperator enum, too
        Addition = 1,
        Subtraction,
        Multiplication,
        Division,
        Modulus,
        And,
        Or,
        Xor,
        Less,
        LessOrEqual,
        Equal,
        Unequal,
        GreaterOrEqual,
        Greater,
        LeftShift,
        RightShift,
        Increment,
        Decrement,
        //If changed, change UnaryOperator enum, too
        Plus = 0x1 <<16,
        Negation = 0x2 << 16,
        Not = 0x3 << 16,
        IsTrue = 0x4 << 16,
        IsFalse = 0x5 << 16
    }
}
