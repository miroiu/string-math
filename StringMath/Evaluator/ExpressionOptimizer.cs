using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    internal class ExpressionOptimizer : IExpressionVisitor<Expression>
    {
        private readonly Dictionary<Type, Func<Expression, Expression>> _expressionOptimizers;
        private readonly IMathContext _context;

        /// <summary>Initializez a new instance of an expression optimizer.</summary>
        /// <param name="mathContext">The math context used to evaluate expressions.</param>
        public ExpressionOptimizer(IMathContext mathContext)
        {
            mathContext.EnsureNotNull(nameof(mathContext));
            _context = mathContext;

            _expressionOptimizers = new Dictionary<Type, Func<Expression, Expression>>
            {
                [typeof(BinaryExpression)] = OptimizeBinaryExpression,
                [typeof(UnaryExpression)] = EvaluateUnaryExpression,
                [typeof(ConstantExpression)] = OptimizeConstantExpression,
                [typeof(GroupingExpression)] = OptimizeGroupingExpression,
                [typeof(VariableExpression)] = SkipExpressionOptimization,
                [typeof(ValueExpression)] = SkipExpressionOptimization
            };
        }
        
        /// <summary>Simplifies an expression tree by removing unnecessary nodes and evaluating constant expressions.</summary>
        /// <param name="expression">The expression tree to optimize.</param>
        /// <returns>An optimized expression tree.</returns>
        public Expression Visit(Expression expression)
        {
            Expression result = _expressionOptimizers[expression.Type](expression);
            return result;
        }

        private Expression OptimizeConstantExpression(Expression expr)
        {
            ConstantExpression constantExpr = (ConstantExpression)expr;
            return constantExpr.ToValueExpression();
        }

        private Expression OptimizeGroupingExpression(Expression expr)
        {
            GroupingExpression groupingExpr = (GroupingExpression)expr;
            Expression innerExpr = Visit(groupingExpr.Inner);
            return innerExpr;
        }

        private Expression EvaluateUnaryExpression(Expression expr)
        {
            UnaryExpression unaryExpr = (UnaryExpression)expr;
            Expression operandExpr = Visit(unaryExpr.Operand);
            if (operandExpr is ValueExpression valueExpr)
            {
                double result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
                return new ValueExpression(result);
            }

            return new UnaryExpression(unaryExpr.OperatorName, operandExpr);
        }

        private Expression OptimizeBinaryExpression(Expression expr)
        {
            BinaryExpression binaryExpr = (BinaryExpression)expr;
            Expression leftExpr = Visit(binaryExpr.Left);
            Expression rightExpr = Visit(binaryExpr.Right);

            if (leftExpr is ValueExpression leftValue && rightExpr is ValueExpression rightValue)
            {
                double result = _context.EvaluateBinary(binaryExpr.OperatorName, leftValue.Value, rightValue.Value);
                return new ValueExpression(result);
            }

            return new BinaryExpression(leftExpr, binaryExpr.OperatorName, rightExpr);
        }

        private Expression SkipExpressionOptimization(Expression expr)
        {
            return expr;
        }
    }
}
