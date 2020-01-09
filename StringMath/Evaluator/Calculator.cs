using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StringMath.Tests")]
namespace StringMath
{
    public static class Calculator
    {
        internal static MathContext MathContext { get; } = new MathContext();

        public static void AddBinaryOperator(string operatorName, Func<Number, Number, Number> operation)
            => MathContext.AddBinaryOperator(operatorName, operation);

        public static void AddUnaryOperator(string operatorName, Func<Number, Number> operation)
            => MathContext.AddUnaryOperator(operatorName, operation);

        private static readonly Dictionary<Type, Func<Expression, Replacement[], Expression>> _expressionEvaluators = new Dictionary<Type, Func<Expression, Replacement[], Expression>>
        {
            [typeof(BinaryExpression)] = EvaluateBinaryExpression,
            [typeof(UnaryExpression)] = EvaluateUnaryExpression,
            [typeof(ConstantExpression)] = EvaluateConstantExpression,
            [typeof(GroupingExpression)] = EvaluateGroupingExpression,
            [typeof(ReplacementExpression)] = EvaluateReplacementExpression
        };

        public static Number Evaluate(string expression)
            => Evaluate(expression, default);

        public static Number Evaluate(string expression, params Replacement[] replacements)
        {
            SourceText text = new SourceText(expression);
            Lexer lex = new Lexer(text, MathContext);
            Parser parse = new Parser(lex, MathContext);

            Expression expr = Reduce(parse.Parse(), replacements);
            return ((NumberExpression)expr).Value;
        }

        internal static Expression Reduce(Expression expression, Replacement[] replacements)
        {
            if (expression is NumberExpression)
            {
                return expression;
            }

            var expr = _expressionEvaluators[expression.Type](expression, replacements);
            return Reduce(expr, replacements);
        }

        private static Expression EvaluateConstantExpression(Expression arg, Replacement[] replacements)
        {
            var constant = (ConstantExpression)arg;
            return new NumberExpression(Number.Parse(constant.Value));
        }

        private static Expression EvaluateGroupingExpression(Expression arg, Replacement[] replacements)
        {
            var grouping = (GroupingExpression)arg;
            return grouping.Inner;
        }

        private static Expression EvaluateUnaryExpression(Expression arg, Replacement[] replacements)
        {
            var unary = (UnaryExpression)arg;
            var value = (NumberExpression)Reduce(unary.Operand, replacements);

            var result = MathContext.EvaluateUnary(unary.OperatorType, value.Value);
            return new NumberExpression(result);
        }

        private static Expression EvaluateBinaryExpression(Expression expr, Replacement[] replacements)
        {
            var binary = (BinaryExpression)expr;
            var left = (NumberExpression)Reduce(binary.Left, replacements);
            var right = (NumberExpression)Reduce(binary.Right, replacements);

            var result = MathContext.EvaluateBinary(binary.OperatorType, left.Value, right.Value);
            return new NumberExpression(result);
        }

        private static Expression EvaluateReplacementExpression(Expression expr, Replacement[] replacements)
        {
            var replacement = (ReplacementExpression)expr;
            var value = replacements.FirstOrDefault(r => r.Identifier == replacement.Name);

            return new NumberExpression(value.Value);
        }
    }
}
