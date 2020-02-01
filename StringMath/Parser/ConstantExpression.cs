using System;

namespace StringMath
{
    internal sealed class ConstantExpression : Expression
    {
        public ConstantExpression(string value)
            => Value = value;

        public string Value { get; }
        public override Type Type => typeof(ConstantExpression);
    }
}