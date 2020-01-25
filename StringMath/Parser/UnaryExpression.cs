using System;

namespace StringMath
{
    internal sealed class UnaryExpression : Expression
    {
        public UnaryExpression(string operatorName, Expression operand)
        {
            OperatorName = operatorName;
            Operand = operand;
        }

        public string OperatorName { get; }
        public Expression Operand { get; }
        public override Type Type => typeof(UnaryExpression);
    }
}
