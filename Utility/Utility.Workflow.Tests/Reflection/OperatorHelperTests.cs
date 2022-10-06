using System;
using System.Collections.Generic;
using System.Text;
using Utility.Reflection;
using Xunit;

namespace Utility.Tests.Reflection
{
    public class OperatorHelperTests
    {
        [Fact]
        public void TestIsBinaryOperator()
        {
            Operators[] unops = new Operators[]{
                Operators.Plus,
                Operators.Negation,
                Operators.Not,
                Operators.IsTrue,
                Operators.IsFalse
            };

            Operators[] binops = new Operators[]{
                Operators.Addition,
                Operators.Subtraction,
                Operators.Multiplication,
                Operators.Division,
                Operators.Modulus,
                Operators.And,
                Operators.Or,
                Operators.Xor,
                Operators.Less,
                Operators.LessOrEqual,
                Operators.Equal,
                Operators.Unequal,
                Operators.GreaterOrEqual,
                Operators.Greater,
                Operators.LeftShift,
                Operators.RightShift,
                Operators.Increment,
                Operators.Decrement,
            };

            foreach (var binop in binops)
                Assert.True(OperatorHelper.IsBinaryOperator(binop));

            foreach (var unop in unops)
                Assert.False(OperatorHelper.IsBinaryOperator(unop));
        }

        [Fact]
        public void TestIsUnaryOperator()
        {
            Operators[] unops = new Operators[]{
                Operators.Plus,
                Operators.Negation,
                Operators.Not,
                Operators.IsTrue,
                Operators.IsFalse
            };

            Operators[] binops = new Operators[]{
                Operators.Addition,
                Operators.Subtraction,
                Operators.Multiplication,
                Operators.Division,
                Operators.Modulus,
                Operators.And,
                Operators.Or,
                Operators.Xor,
                Operators.Less,
                Operators.LessOrEqual,
                Operators.Equal,
                Operators.Unequal,
                Operators.GreaterOrEqual,
                Operators.Greater,
                Operators.LeftShift,
                Operators.RightShift,
                Operators.Increment,
                Operators.Decrement,
            };

            foreach (var binop in binops)
                Assert.False(OperatorHelper.IsUnaryOperator(binop));

            foreach (var unop in unops)
                Assert.True(OperatorHelper.IsUnaryOperator(unop));
        }

        [Fact]
        public void TestGetOperatorName()
        {
            Operators[] binops = new Operators[]{
                Operators.Addition ,
                Operators.Subtraction,
                Operators.Multiplication,
                Operators.Division,
                Operators.Modulus,
                Operators.And,
                Operators.Or,
                Operators.Xor,
                Operators.Less,
                Operators.LessOrEqual,
                Operators.Equal,
                Operators.Unequal,
                Operators.GreaterOrEqual,
                Operators.Greater,
                Operators.LeftShift,
                Operators.RightShift,
                Operators.Increment,
                Operators.Decrement,
            };

            Operators[] unops = new Operators[]{
                Operators.Plus,
                Operators.Negation,
                Operators.Not,
                Operators.IsTrue,
                Operators.IsFalse
            };
            string[] binopNames = new string[]{
                "op_Addition",
                "op_Subtraction",
                "op_Multiply",
                "op_Division",
                "op_Modulus",
                "op_BitwiseAnd",
                "op_BitwiseOr",
                "op_ExclusiveOr",
                "op_LessThan",
                "op_LessThanOrEqual",
                "op_Equality",
                "op_Inequality",
                "op_GreaterThanOrEqual",
                "op_GreaterThan",
                "op_LeftShift",
                "op_RightShift",
                "op_Increment",
                "op_Decrement"
            };

            string[] unopNames = new string[]{
                "op_UnaryPlus",
                "op_UnaryNegation",
                "op_LogicalNot",
                "op_True",
                "op_False"
            };

            for (int i = 0; i < unopNames.Length; i++)
                Assert.Equal(unopNames[i], OperatorHelper.GetOperatorName(unops[i]));

            for (int i = 0; i < binopNames.Length; i++)
                Assert.Equal(binopNames[i], OperatorHelper.GetOperatorName(binops[i]));

            for (int i = 0; i < unopNames.Length; i++)
                Assert.Equal(unopNames[i], OperatorHelper.GetOperatorName(unops[i]));

            for (int i = 0; i < binopNames.Length; i++)
                Assert.Equal(binopNames[i], OperatorHelper.GetOperatorName(binops[i]));
        }

        [Fact]
        public void TestConvert()
        {
            foreach (BinaryOperator binop in Enum.GetValues(typeof(BinaryOperator)))
            {
                Assert.Equal(binop.ToString(), OperatorHelper.Convert(binop).ToString());
            }

            foreach (UnaryOperator unop in Enum.GetValues(typeof(UnaryOperator)))
            {
                Assert.Equal(unop.ToString(), OperatorHelper.Convert(unop).ToString());
            }

            foreach (Operators op in Enum.GetValues(typeof(Operators)))
            {
                UnaryOperator unop;
                BinaryOperator binop;

                OperatorHelper.Convert(op, out unop, out binop);

                if (OperatorHelper.IsUnaryOperator(op))
                    Assert.Equal(op.ToString(), unop.ToString());

                if (OperatorHelper.IsBinaryOperator(op))
                    Assert.Equal(op.ToString(), binop.ToString());
            }
        }
    }
}
