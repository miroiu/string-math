using System.Globalization;

namespace StringMath.Expressions
{
    /// <summary>A constant expression.</summary>
    internal sealed class ConstantExpression : IExpression
    {
        /// <summary>Initializes a new instance of a constant expression.</summary>
        /// <param name="value">The value of the constant.</param>
        public ConstantExpression(string value)
        {
            Value = double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>Initializes a new instance of a constant expression.</summary>
        /// <param name="value">The value of the constant.</param>
        public ConstantExpression(double value)
        {
            Value = value;
        }

        /// <summary>The constant value.</summary>
        public double Value { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.ConstantExpression;

        /// <inheritdoc />
        public override string ToString()
             => ToString(MathContext.Default);

        public string ToString(IMathContext context)
            => Value.ToString();
    }
}