using System;

namespace StringMath
{
    internal sealed class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, string operatorName, Expression right)
        {
            Left = left;
            OperatorName = operatorName;
            Right = right;
        }

        public Expression Left { get; }
        public string OperatorName { get; }
        public Expression Right { get; }
        public override Type Type => typeof(BinaryExpression);
    }
}
