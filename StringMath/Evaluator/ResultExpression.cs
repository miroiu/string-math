using System;

namespace StringMath
{
    internal sealed class ResultExpression : Expression
    {
        public ResultExpression(decimal number)
            => Value = number;

        public decimal Value { get; }
        public override Type Type => typeof(ResultExpression);
    }
}
