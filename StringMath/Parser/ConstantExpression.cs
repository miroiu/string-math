using System;

namespace StringMath
{
    internal class ConstantExpression : Expression
    {
        public string Value { get; }

        public ConstantExpression(string value)
            => Value = value;

        public override Type Type => typeof(ConstantExpression);
    }
}