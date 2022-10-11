using StringMath.Expressions;

namespace StringMath
{
    internal abstract class BaseExpressionVisitor : IExpressionVisitor
    {
        public IExpression Visit(IExpression expression)
        {
            IExpression result = expression switch
            {
                BinaryExpression binaryExpr => VisitBinaryExpr(binaryExpr),
                ConstantExpression constantExpr => VisitConstantExpr(constantExpr),
                UnaryExpression unaryExpr => VisitUnaryExpr(unaryExpr),
                VariableExpression variableExpr => VisitVariableExpr(variableExpr),
                _ => expression
            };

            return result;
        }

        protected virtual IExpression VisitVariableExpr(VariableExpression variableExpr) => variableExpr;

        protected virtual IExpression VisitConstantExpr(ConstantExpression constantExpr) => constantExpr;

        protected virtual IExpression VisitBinaryExpr(BinaryExpression binaryExpr) => binaryExpr;

        protected virtual IExpression VisitUnaryExpr(UnaryExpression unaryExpr) => unaryExpr;
    }
}
