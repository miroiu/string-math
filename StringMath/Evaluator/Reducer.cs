using System;
using System.Collections.Generic;

namespace StringMath
{
    internal sealed class Reducer
    {
        private readonly Dictionary<Type, Func<Expression, Expression>> _expressionEvaluators;
        private Dictionary<string, decimal> _replacements;
        private MathContext _context;

        public Reducer()
        {
            _expressionEvaluators = new Dictionary<Type, Func<Expression, Expression>>
            {
                [typeof(BinaryExpression)] = EvaluateBinaryExpression,
                [typeof(UnaryExpression)] = EvaluateUnaryExpression,
                [typeof(ConstantExpression)] = EvaluateConstantExpression,
                [typeof(GroupingExpression)] = EvaluateGroupingExpression,
                [typeof(ReplacementExpression)] = EvaluateReplacementExpression
            };
        }

        public T Reduce<T>(Expression expression, MathContext context, Dictionary<string, decimal> replacements) where T : Expression
        {
            _context = context;
            _replacements = replacements ?? new Dictionary<string, decimal>();
            return Reduce<T>(expression);
        }

        #region Evaluators

        private T Reduce<T>(Expression expression)
        {
            if (expression is T expected)
            {
                return expected;
            }

            return Reduce<T>(_expressionEvaluators[expression.Type](expression));
        }

        private Expression EvaluateConstantExpression(Expression arg)
            => new ResultExpression(decimal.Parse(((ConstantExpression)arg).Value));

        private Expression EvaluateGroupingExpression(Expression arg)
            => ((GroupingExpression)arg).Inner;

        private Expression EvaluateUnaryExpression(Expression arg)
        {
            var unary = (UnaryExpression)arg;
            var value = Reduce<ResultExpression>(unary.Operand);

            var result = _context.EvaluateUnary(unary.OperatorName, value.Value);
            return new ResultExpression(result);
        }

        private Expression EvaluateBinaryExpression(Expression expr)
        {
            var binary = (BinaryExpression)expr;
            var left = Reduce<ResultExpression>(binary.Left);
            var right = Reduce<ResultExpression>(binary.Right);

            var result = _context.EvaluateBinary(binary.OperatorName, left.Value, right.Value);
            return new ResultExpression(result);
        }

        private Expression EvaluateReplacementExpression(Expression expr)
        {
            var replacement = (ReplacementExpression)expr;
            if (_replacements.TryGetValue(replacement.Name, out decimal value))
            {
                return new ResultExpression(value);
            }

            throw new LangException($"A value was not supplied for variable '{replacement.Name}'.");
        }

        #endregion
    }
}
