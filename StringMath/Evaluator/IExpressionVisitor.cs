namespace StringMath
{
    /// <summary>Contract for expression visitors.</summary>
    internal interface IExpressionVisitor<T> where T : Expression
    {
        /// <summary>Visits an expression tree and transforms it to another of type <typeparamref name="T"/>.</summary>
        /// <param name="expression">The expression to transform.</param>
        /// <returns>An expression of type <typeparamref name="T"/>.</returns>
        T Visit(Expression expression);
    }
}
