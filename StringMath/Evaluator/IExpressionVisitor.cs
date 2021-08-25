namespace StringMath
{
    /// <summary>Contract for expression visitors.</summary>
    internal interface IExpressionVisitor<out T> where T : IExpression
    {
        /// <summary>Visits an expression tree and transforms it to another of type <typeparamref name="T"/>.</summary>
        /// <param name="expression">The expression to transform.</param>
        /// <returns>An expression of type <typeparamref name="T"/>.</returns>
        T Visit(IExpression expression);
    }
}
