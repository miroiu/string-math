using System;
using System.Globalization;

namespace StringMath
{
    /// <summary>A value expression.</summary>
    internal sealed class ValueExpression<TNum> : IExpression where TNum : INumber<TNum>
    {
        /// <summary>Initializes an instance of a value expression.</summary>
        /// <param name="value">The value of the expression.</param>
        public ValueExpression(TNum value)
            => Value = value;

        /// <summary>The value of the expression.</summary>
        public TNum Value { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.ValueExpression;

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
