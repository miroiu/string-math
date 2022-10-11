namespace StringMath.Expressions
{
    /// <summary>Available expression types.</summary>
    internal enum ExpressionType
    {
        /// <summary><see cref="Expressions.UnaryExpression"/>.</summary>
        UnaryExpression,
        /// <summary><see cref="Expressions.BinaryExpression"/>.</summary>
        BinaryExpression,
        /// <summary><see cref="Expressions.VariableExpression"/>.</summary>
        VariableExpression,
        /// <summary><see cref="Expressions.ConstantExpression"/>.</summary>
        ConstantExpression,
    }

    /// <summary>Contract for expressions.</summary>
    internal interface IExpression
    {
        /// <summary>The type of the expression.</summary>
        ExpressionType Type { get; }
    }
}
