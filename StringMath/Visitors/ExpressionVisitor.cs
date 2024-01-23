using StringMath.Expressions;

namespace StringMath
{
    /// <summary>Contract for expression visitors.</summary>
    internal abstract class ExpressionVisitor
    {
        /// <summary>Visits an expression tree and transforms it into another expression tree.</summary>
        /// <param name="expression">The expression to transform.</param>
        /// <returns>A new expression tree.</returns>
        public IExpression Visit(IExpression expression)
        {
            IExpression result = expression switch
            {
                BinaryExpression binaryExpr => VisitBinaryExpr(binaryExpr),
                ConstantExpression constantExpr => VisitConstantExpr(constantExpr),
                UnaryExpression unaryExpr => VisitUnaryExpr(unaryExpr),
                VariableExpression variableExpr => VisitVariableExpr(variableExpr),
                InvocationExpression invocationExpr => VisitInvocationExpr(invocationExpr),
                _ => expression
            };

            return result;
        }

        protected virtual IExpression VisitVariableExpr(VariableExpression variableExpr) => variableExpr;

        protected virtual IExpression VisitConstantExpr(ConstantExpression constantExpr) => constantExpr;

        protected virtual IExpression VisitBinaryExpr(BinaryExpression binaryExpr) => binaryExpr;

        protected virtual IExpression VisitUnaryExpr(UnaryExpression unaryExpr) => unaryExpr;

        protected virtual IExpression VisitInvocationExpr(InvocationExpression invocationExpr) => invocationExpr;
    }
}
