using System;
using System.Collections.Generic;

namespace StringMath
{
    internal sealed class Reducer
    {
        private readonly Dictionary<Type, Func<Expression, Expression>> _expressionEvaluators;
        private Dictionary<string, double> _replacements;
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

        public T Reduce<T>(Expression expression, MathContext context, Dictionary<string, double> replacements) where T : Expression
        {
            _context = context;
            _replacements = replacements ?? new Dictionary<string, double>();
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
        {
            return new ResultExpression(double.Parse(((ConstantExpression)arg).Value));
        }

        private Expression EvaluateGroupingExpression(Expression arg)
        {
            return ((GroupingExpression)arg).Inner;
        }

        private Expression EvaluateUnaryExpression(Expression arg)
        {
            UnaryExpression unary = (UnaryExpression)arg;
            ResultExpression value = Reduce<ResultExpression>(unary.Operand);

            double result = _context.EvaluateUnary(unary.OperatorName, value.Value);
            return new ResultExpression(result);
        }

        private Expression EvaluateBinaryExpression(Expression expr)
        {
            BinaryExpression binary = (BinaryExpression)expr;
            ResultExpression left = Reduce<ResultExpression>(binary.Left);
            ResultExpression right = Reduce<ResultExpression>(binary.Right);

            double result = _context.EvaluateBinary(binary.OperatorName, left.Value, right.Value);
            return new ResultExpression(result);
        }

        private Expression EvaluateReplacementExpression(Expression expr)
        {
            ReplacementExpression replacement = (ReplacementExpression)expr;
            return _replacements.TryGetValue(replacement.Name, out double value)
                ? new ResultExpression(value)
                : throw new LangException($"A value was not supplied for variable '{replacement.Name}'.");
        }

        #endregion
    }
}
