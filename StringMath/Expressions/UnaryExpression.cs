using System;

namespace StringMath.Expressions
{
    /// <summary>An unary expression.</summary>
    internal sealed class UnaryExpression : IExpression
    {
        /// <summary>Initializes a new instance of an unary expression.</summary>
        /// <param name="operatorName">The unary operator's name.</param>
        /// <param name="operand">The operand expression.</param>
        public UnaryExpression(string operatorName, IExpression operand)
        {
            OperatorName = operatorName;
            Operand = operand;
        }

        /// <summary>The unary operator's name.</summary>
        public string OperatorName { get; }

        /// <summary>The operand expression.</summary>
        public IExpression Operand { get; }

        /// <inheritdoc />
        public ExpressionType Type => ExpressionType.UnaryExpression;

        /// <inheritdoc />
        public override string ToString()
            => ToString(MathContext.Default);

        public string ToString(IMathContext context)
            => OperatorName.Length > 2 || Operand is BinaryExpression || Operand is UnaryExpression
                ? $"{OperatorName}({Operand.ToString(context)})"
                : string.Equals(OperatorName, "!", StringComparison.Ordinal)
                ? $"{Operand.ToString(context)}{OperatorName}"
                : $"{OperatorName}{Operand.ToString(context)}";
    }
}
