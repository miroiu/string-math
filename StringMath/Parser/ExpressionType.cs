namespace StringMath
{
    /// <summary>Available expression types.</summary>
    internal enum ExpressionType
    {
        /// <summary><see cref="StringMath.UnaryExpression"/>.</summary>
        UnaryExpression,
        /// <summary><see cref="StringMath.BinaryExpression"/>.</summary>
        BinaryExpression,
        /// <summary><see cref="StringMath.GroupingExpression"/>.</summary>
        GroupingExpression,
        /// <summary><see cref="StringMath.VariableExpression"/>.</summary>
        VariableExpression,
        /// <summary><see cref="StringMath.ConstantExpression"/>.</summary>
        ConstantExpression,
        /// <summary><see cref="StringMath.ValueExpression"/>.</summary>
        ValueExpression,
    }
}
