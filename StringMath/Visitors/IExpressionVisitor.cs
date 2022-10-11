using StringMath.Expressions;

namespace StringMath
{
    /// <summary>Contract for expression visitors.</summary>
    internal interface IExpressionVisitor
    {
        /// <summary>Visits an expression tree and transforms it into another expression tree.</summary>
        /// <param name="expression">The expression to transform.</param>
        /// <returns>A new expression tree.</returns>
        IExpression Visit(IExpression expression);
    }
}
