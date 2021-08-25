using System;

namespace StringMath
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
        {
            return $"{Left} {OperatorName} {Right}";
        }
    }
}
