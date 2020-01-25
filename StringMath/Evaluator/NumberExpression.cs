using System;

namespace StringMath
{
    internal sealed class NumberExpression : Expression
    {
        public NumberExpression(decimal number)
            => Value = number;

        public decimal Value { get; }

        public override Type Type => typeof(NumberExpression);
    }
}
