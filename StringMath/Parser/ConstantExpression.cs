namespace StringMath
{
    /// <summary>A constant expression.</summary>
    internal sealed class ConstantExpression : IExpression
    {
        /// <summary>Initializez a new instance of a constant expression.</summary>
        /// <param name="value">The value of the constant.</param>
        public ConstantExpression(string value)
            => Value = value;

        /// <summary>The constant value.</summary>
        public string Value { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.ConstantExpression;

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }
    }
}