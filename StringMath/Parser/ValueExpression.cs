using System;
using System.Globalization;

namespace StringMath
{
    internal sealed class ValueExpression : Expression
    {
        public ValueExpression(double number)
            => Value = number;

        public double Value { get; }
        public override Type Type => typeof(ValueExpression);

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
