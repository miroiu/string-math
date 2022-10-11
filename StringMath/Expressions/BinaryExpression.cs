namespace StringMath.Expressions
{
    /// <summary>A binary expression.</summary>
    internal sealed class BinaryExpression : IExpression
    {
        /// <summary>Initializez a new instance of a binary expression.</summary>
        /// <param name="left">The left expression tree.</param>
        /// <param name="operatorName">The binary operator's name.</param>
        /// <param name="right">The right expression tree.</param>
        public BinaryExpression(IExpression left, string operatorName, IExpression right)
        {
            Left = left;
            OperatorName = operatorName;
            Right = right;
        }

        /// <summary>The left expression tree.</summary>
        public IExpression Left { get; }

        /// <summary>The binary operator's name.</summary>
        public string OperatorName { get; }

        /// <summary>The right expression tree.</summary>
        public IExpression Right { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.BinaryExpression;

        /// <inheritdoc />
        public override string ToString()
            => ToString(MathContext.Default);

        public string ToString(IMathContext context)
        {
            bool addLeft = Left is BinaryExpression left && context.GetBinaryPrecedence(OperatorName) > context.GetBinaryPrecedence(left.OperatorName);
            bool addRight = Right is BinaryExpression right && context.GetBinaryPrecedence(OperatorName) > context.GetBinaryPrecedence(right.OperatorName);

            string? leftStr = addLeft ? $"({Left.ToString(context)})" : Left.ToString(context);
            string? rightStr = addRight ? $"({Right.ToString(context)})" : Right.ToString(context);

            return $"{leftStr} {OperatorName} {rightStr}";
        }
    }
}
