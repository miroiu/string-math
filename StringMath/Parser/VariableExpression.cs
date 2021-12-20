namespace StringMath
{
    /// <summary>A variable expression.</summary>
    internal sealed class VariableExpression : IExpression
    {
        /// <summary>Initializes a new instance of a variable expression.</summary>
        /// <param name="name">The variable name.</param>
        public VariableExpression(string name)
#if NETCOREAPP || NETSTANDARD
            => Name = name[1..^1];
#else
            => Name = name.Substring(1, name.Length - 2);
#endif

        /// <summary>The variable name.</summary>
        public string Name { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.VariableExpression;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{{{Name}}}";
        }
    }
}
