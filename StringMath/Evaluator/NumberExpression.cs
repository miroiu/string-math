using System;

namespace StringMath
{
    internal class NumberExpression : Expression
    {
        public NumberExpression(decimal number)
            => Value = number;

        public decimal Value { get; }

        public override Type Type => typeof(NumberExpression);
    }
}
