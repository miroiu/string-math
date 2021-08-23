using System;

namespace StringMath
{
    internal sealed class ResultExpression : Expression
    {
        public ResultExpression(double number)
            => Value = number;

        public double Value { get; }
        public override Type Type => typeof(ResultExpression);
    }
}
