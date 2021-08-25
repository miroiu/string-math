using System.Globalization;

namespace StringMath
{
    /// <summary>A value expression.</summary>
    internal sealed class ValueExpression : IExpression
    {
        /// <summary>Initializes an instance of a value expression.</summary>
        /// <param name="value">The value of the expression.</param>
        public ValueExpression(double value)
            => Value = value;

        /// <summary>The value of the expression.</summary>
        public double Value { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.ValueExpression;

        /// <inheritdoc />
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
