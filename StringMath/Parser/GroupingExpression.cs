namespace StringMath
{
    /// <summary>A grouping expression.</summary>
    internal sealed class GroupingExpression : IExpression
    {
        /// <summary>Initializez a new instance of a constant expression.</summary>
        /// <param name="inner">The inner expression.</param>
        public GroupingExpression(IExpression inner)
            => Inner = inner;

        /// <summary>The inner expression.</summary>
        public IExpression Inner { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.GroupingExpression;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({Inner})";
        }
    }
}
