using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Reflection
{
    public static class OperatorHelper
    {
        public static bool IsBinaryOperator(Operators op)
        {
            return ((int)op & ~0xffff) == 0;
        }

        public static bool IsUnaryOperator(Operators op)
        {
            return ((int)op & ~0xffff) != 0;
        }

        public static string GetOperatorName(UnaryOperator unaryOperator) => GetOperatorName(Convert(unaryOperator));

        public static string GetOperatorName(BinaryOperator binaryOperator) => GetOperatorName(Convert(binaryOperator));

        public static string GetOperatorName(Operators op)
        {
            if (op == Operators.Addition)
            {
                return "op_Addition";
            }
            else if (op == Operators.And)
            {
                return "op_BitwiseAnd";
            }
            else if (op == Operators.Decrement)
            {
                return "op_Decrement";
            }
            else if (op == Operators.Division)
            {
                return "op_Division";
            }
            else if (op == Operators.Equal)
            {
                return "op_Equality";
            }
            else if (op == Operators.Greater)
            {
                return "op_GreaterThan";
            }
            else if (op == Operators.GreaterOrEqual)
            {
                return "op_GreaterThanOrEqual";
            }
            else if (op == Operators.Increment)
            {
                return "op_Increment";
            }
            else if (op == Operators.IsFalse)
            {
                return "op_False";
            }
            else if (op == Operators.IsTrue)
            {
                return "op_True";
            }
            else if (op == Operators.LeftShift)
            {
                return "op_LeftShift";
            }
            else if (op == Operators.Less)
            {
                return "op_LessThan";
            }
            else if (op == Operators.LessOrEqual)
            {
                return "op_LessThanOrEqual";
            }
            else if (op == Operators.Not)
            {
                return "op_LogicalNot";
            }
            else if (op == Operators.Modulus)
            {
                return "op_Modulus";
            }
            else if (op == Operators.Multiplication)
            {
                return "op_Multiply";
            }
            else if (op == Operators.Negation)
            {
                return "op_UnaryNegation";
            }
            else if (op == Operators.Or)
            {
                return "op_BitwiseOr";
            }
            else if (op == Operators.Plus)
            {
                return "op_UnaryPlus";
            }
            else if (op == Operators.RightShift)
            {
                return "op_RightShift";
            }
            else if (op == Operators.Subtraction)
            {
                return "op_Subtraction";
            }
            else if (op == Operators.Unequal)
            {
                return "op_Inequality";
            }
            else if (op == Operators.Xor)
            {
                return "op_ExclusiveOr";
            }
            else
                throw new ArgumentException("Unsupported operation", nameof(op));
        }

        public static void Convert(Operators op, out UnaryOperator unop, out BinaryOperator binop)
        {
            unop = (UnaryOperator)((int)op >> 16);
            binop = (BinaryOperator)((int)op & 0xffff);
        }

        public static Operators Convert(UnaryOperator op)
        {
            return (Operators)((int)op << 16);
        }

        public static Operators Convert(BinaryOperator op)
        {
            return (Operators)op;
        }
    }
}
