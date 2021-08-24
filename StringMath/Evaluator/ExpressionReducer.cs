using System;
using System.Collections.Generic;

namespace StringMath
{
    internal interface IExpressionReducer
    {
        T Reduce<T>(Expression expression, IMathContext context, IVariablesCollection variables) where T : Expression;
    }

    internal sealed class ExpressionReducer : IExpressionReducer
    {
        private readonly Dictionary<Type, Func<Expression, Expression>> _expressionEvaluators;
        private IVariablesCollection _variables = default!;
        private IMathContext _context = default!;

        public ExpressionReducer()
        {
            _expressionEvaluators = new Dictionary<Type, Func<Expression, Expression>>
            {
                [typeof(BinaryExpression)] = EvaluateBinaryExpression,
                [typeof(UnaryExpression)] = EvaluateUnaryExpression,
                [typeof(ConstantExpression)] = EvaluateConstantExpression,
                [typeof(GroupingExpression)] = EvaluateGroupingExpression,
                [typeof(VariableExpression)] = EvaluateVariableExpression
            };
        }

        public T Reduce<T>(Expression expression, IMathContext context, IVariablesCollection variables) where T : Expression
        {
            _context = context;
            _variables = variables ?? new VariablesCollection();
            return Reduce<T>(expression);
        }

        #region Evaluators

        private T Reduce<T>(Expression expression)
        {
            return expression is T expected ? expected : Reduce<T>(_expressionEvaluators[expression.Type](expression));
        }

        private Expression EvaluateConstantExpression(Expression arg)
        {
            return new ValueExpression(double.Parse(((ConstantExpression)arg).Value));
        }

        private Expression EvaluateGroupingExpression(Expression arg)
        {
            return ((GroupingExpression)arg).Inner;
        }

        private Expression EvaluateUnaryExpression(Expression arg)
        {
            UnaryExpression unary = (UnaryExpression)arg;
            ValueExpression value = Reduce<ValueExpression>(unary.Operand);

            double result = _context.EvaluateUnary(unary.OperatorName, value.Value);
            return new ValueExpression(result);
        }

        private Expression EvaluateBinaryExpression(Expression expr)
        {
            BinaryExpression binary = (BinaryExpression)expr;
            ValueExpression left = Reduce<ValueExpression>(binary.Left);
            ValueExpression right = Reduce<ValueExpression>(binary.Right);

            double result = _context.EvaluateBinary(binary.OperatorName, left.Value, right.Value);
            return new ValueExpression(result);
        }

        private Expression EvaluateVariableExpression(Expression expr)
        {
            VariableExpression variable = (VariableExpression)expr;
            return _variables.TryGetValue(variable.Name, out double value)
                ? new ValueExpression(value)
                : throw new LangException($"A value was not supplied for variable '{variable.Name}'.");
        }

        #endregion
    }
}
