using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    internal sealed class ExpressionEvaluator : IExpressionVisitor<ValueExpression>
    {
        private readonly Dictionary<ExpressionType, Func<IExpression, ValueExpression>> _expressionEvaluators;
        private readonly IMathContext _context;
        private readonly IVariablesCollection _variables;

        /// <summary>Initializez a new instance of an expression evaluator.</summary>
        /// <param name="context">The math context.</param>
        /// <param name="variables">The variables collection.</param>
        public ExpressionEvaluator(IMathContext context, IVariablesCollection variables)
        {
            context.EnsureNotNull(nameof(context));
            variables.EnsureNotNull(nameof(variables));

            _variables = variables;
            _context = context;

            _expressionEvaluators = new Dictionary<ExpressionType, Func<IExpression, ValueExpression>>
            {
                [ExpressionType.BinaryExpression] = EvaluateBinaryExpression,
                [ExpressionType.UnaryExpression] = EvaluateUnaryExpression,
                [ExpressionType.ConstantExpression] = EvaluateConstantExpression,
                [ExpressionType.GroupingExpression] = EvaluateGroupingExpression,
                [ExpressionType.VariableExpression] = EvaluateVariableExpression
            };
        }

        /// <summary>Evaluates an expression tree and returns the resulting value.</summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>An value expression.</returns>
        public ValueExpression Visit(IExpression expression)
        {
            if (expression is ValueExpression expected)
            {
                return expected;
            }

            ValueExpression result = _expressionEvaluators[expression.Type](expression);
            return result;
        }

        private ValueExpression EvaluateConstantExpression(IExpression expr)
        {
            ConstantExpression constantExpr = (ConstantExpression)expr;
            ValueExpression valueExpr = constantExpr.ToValueExpression();
            return valueExpr;
        }

        private ValueExpression EvaluateGroupingExpression(IExpression expr)
        {
            GroupingExpression groupingExpr = (GroupingExpression)expr;
            ValueExpression innerExpr = Visit(groupingExpr.Inner);
            return innerExpr;
        }

        private ValueExpression EvaluateUnaryExpression(IExpression expr)
        {
            UnaryExpression unaryExpr = (UnaryExpression)expr;
            ValueExpression valueExpr = Visit(unaryExpr.Operand);

            double result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
            return new ValueExpression(result);
        }

        private ValueExpression EvaluateBinaryExpression(IExpression expr)
        {
            BinaryExpression binaryExpr = (BinaryExpression)expr;
            ValueExpression leftExpr = Visit(binaryExpr.Left);
            ValueExpression rightExpr = Visit(binaryExpr.Right);

            double result = _context.EvaluateBinary(binaryExpr.OperatorName, leftExpr.Value, rightExpr.Value);
            return new ValueExpression(result);
        }

        private ValueExpression EvaluateVariableExpression(IExpression expr)
        {
            VariableExpression variableExpr = (VariableExpression)expr;
            return _variables.TryGetValue(variableExpr.Name, out double value)
                ? new ValueExpression(value)
                : throw MathException.UnassignedVariable(variableExpr);
        }
    }
}
