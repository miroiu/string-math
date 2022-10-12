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

        /// <summary>Creates a string representation of the expression in the provided context.</summary>
        /// <param name="context">The context in which this expression is printed.</param>
        /// <returns>A string representation of the expression.</returns>
        string ToString(IMathContext context);
    }
}
