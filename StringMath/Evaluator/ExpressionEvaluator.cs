using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    internal sealed class ExpressionEvaluator<TNum> : IExpressionVisitor<ValueExpression<TNum>> where TNum : INumber<TNum>
    {
        private readonly Dictionary<ExpressionType, Func<IExpression, ValueExpression<TNum>>> _expressionEvaluators;
        private readonly IMathContext<TNum> _context;
        private readonly IVariablesCollection<TNum> _variables;

        /// <summary>Initializez a new instance of an expression evaluator.</summary>
        /// <param name="context">The math context.</param>
        /// <param name="variables">The variables collection.</param>
        public ExpressionEvaluator(IMathContext<TNum> context, IVariablesCollection<TNum> variables)
        {
            context.EnsureNotNull(nameof(context));
            variables.EnsureNotNull(nameof(variables));

            _variables = variables;
            _context = context;

            _expressionEvaluators = new Dictionary<ExpressionType, Func<IExpression, ValueExpression<TNum>>>
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
        public ValueExpression<TNum> Visit(IExpression expression)
        {
            if (expression is ValueExpression<TNum> expected)
            {
                return expected;
            }

            ValueExpression<TNum> result = _expressionEvaluators[expression.Type](expression);
            return result;
        }

        private ValueExpression<TNum> EvaluateConstantExpression(IExpression expr)
        {
            ConstantExpression constantExpr = (ConstantExpression)expr;
            ValueExpression<TNum> valueExpr = constantExpr.ToValueExpression<TNum>();
            return valueExpr;
        }

        private ValueExpression<TNum> EvaluateGroupingExpression(IExpression expr)
        {
            GroupingExpression groupingExpr = (GroupingExpression)expr;
            ValueExpression<TNum> innerExpr = Visit(groupingExpr.Inner);
            return innerExpr;
        }

        private ValueExpression<TNum> EvaluateUnaryExpression(IExpression expr)
        {
            UnaryExpression unaryExpr = (UnaryExpression)expr;
            ValueExpression<TNum> valueExpr = Visit(unaryExpr.Operand);

            TNum result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
            return new ValueExpression<TNum>(result);
        }

        private ValueExpression<TNum> EvaluateBinaryExpression(IExpression expr)
        {
            BinaryExpression binaryExpr = (BinaryExpression)expr;
            ValueExpression<TNum> leftExpr = Visit(binaryExpr.Left);
            ValueExpression<TNum> rightExpr = Visit(binaryExpr.Right);

            TNum result = _context.EvaluateBinary(binaryExpr.OperatorName, leftExpr.Value, rightExpr.Value);
            return new ValueExpression<TNum>(result);
        }

        private ValueExpression<TNum> EvaluateVariableExpression(IExpression expr)
        {
            VariableExpression variableExpr = (VariableExpression)expr;
            return _variables.TryGetValue(variableExpr.Name, out TNum value)
                ? new ValueExpression<TNum>(value)
                : throw LangException.UnassignedVariable(variableExpr);
        }
    }
}
