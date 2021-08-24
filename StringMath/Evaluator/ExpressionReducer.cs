using System;
using System.Collections.Generic;

namespace StringMath
{
    internal interface IExpressionReducer
    {
        T Reduce<T>(Expression expression) where T : Expression;
    }

    internal sealed class ExpressionReducer : IExpressionReducer
    {
        private readonly Dictionary<Type, Func<Expression, Expression>> _expressionEvaluators;
        private readonly IMathContext _context;
        private readonly IVariablesCollection _variables;

        public ExpressionReducer(IMathContext context, IVariablesCollection variables)
        {
            context.EnsureNotNull(nameof(context));
            variables.EnsureNotNull(nameof(variables));

            _variables = variables;
            _context = context;

            _expressionEvaluators = new Dictionary<Type, Func<Expression, Expression>>
            {
                [typeof(BinaryExpression)] = EvaluateBinaryExpression,
                [typeof(UnaryExpression)] = EvaluateUnaryExpression,
                [typeof(ConstantExpression)] = EvaluateConstantExpression,
                [typeof(GroupingExpression)] = EvaluateGroupingExpression,
                [typeof(VariableExpression)] = EvaluateVariableExpression
            };
        }

        public T Reduce<T>(Expression expression) where T : Expression
        {
            return expression is T expected ? expected : Reduce<T>(_expressionEvaluators[expression.Type](expression));
        }

        #region Evaluators

        private Expression EvaluateConstantExpression(Expression arg)
        {
            ConstantExpression constantExpr = ((ConstantExpression)arg);
            double value = double.Parse(constantExpr.Value);
            return new ValueExpression(value);
        }

        private Expression EvaluateGroupingExpression(Expression arg)
        {
            GroupingExpression groupingExpr = ((GroupingExpression)arg);
            return groupingExpr.Inner;
        }

        private Expression EvaluateUnaryExpression(Expression arg)
        {
            UnaryExpression unaryExpr = (UnaryExpression)arg;
            ValueExpression valueExpr = Reduce<ValueExpression>(unaryExpr.Operand);

            double result = _context.EvaluateUnary(unaryExpr.OperatorName, valueExpr.Value);
            return new ValueExpression(result);
        }

        private Expression EvaluateBinaryExpression(Expression expr)
        {
            BinaryExpression binaryExpr = (BinaryExpression)expr;
            ValueExpression leftExpr = Reduce<ValueExpression>(binaryExpr.Left);
            ValueExpression rightExpr = Reduce<ValueExpression>(binaryExpr.Right);

            double result = _context.EvaluateBinary(binaryExpr.OperatorName, leftExpr.Value, rightExpr.Value);
            return new ValueExpression(result);
        }

        private Expression EvaluateVariableExpression(Expression expr)
        {
            VariableExpression variableExpr = (VariableExpression)expr;
            return _variables.TryGetValue(variableExpr.Name, out double value)
                ? new ValueExpression(value)
                : throw new LangException($"A value was not supplied for variable '{variableExpr.Name}'.");
        }

        #endregion
    }
}
