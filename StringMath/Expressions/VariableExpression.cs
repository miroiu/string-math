namespace StringMath.Expressions
{
    /// <summary>A variable expression.</summary>
    internal sealed class VariableExpression : IExpression
    {
        /// <summary>Initializes a new instance of a variable expression.</summary>
        /// <param name="name">The variable name.</param>
        public VariableExpression(string name)
            => Name = name;

        /// <summary>The variable name.</summary>
        public string Name { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.VariableExpression;

        /// <inheritdoc />
        public override string ToString()
            => ToString(MathContext.Default);

        public string ToString(IMathContext context)
            => $"{{{Name}}}";
    }
}
