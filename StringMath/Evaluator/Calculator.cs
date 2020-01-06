using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StringMath.Tests")]
namespace StringMath
{
    public static class Calculator
    {
        private static readonly Dictionary<Type, Func<Expression, Expression>> _evaluators = new Dictionary<Type, Func<Expression, Expression>>
        {
            [typeof(BinaryExpression)] = EvaluateBinaryExpression,
            [typeof(UnaryExpression)] = EvaluateUnaryExpression,
            [typeof(ConstantExpression)] = EvaluateConstantExpression,
            [typeof(GroupingExpression)] = EvaluateGroupingExpression,
        };

        private static readonly Dictionary<string, Func<Number, Number, Number>> _binaryEvaluators = new Dictionary<string, Func<Number, Number, Number>>
        {
            ["+"] = (a, b) => a + b,
            ["-"] = (a, b) => a - b,
            ["*"] = (a, b) => a * b,
            ["/"] = (a, b) => a / b,
            ["^"] = (a, b) => (Number)Math.Pow((double)a, (double)b),
        };

        private static readonly Dictionary<string, Func<Number, Number>> _unaryEvaluators = new Dictionary<string, Func<Number, Number>>
        {
            ["-"] = a => -a,
            ["!"] = a => ComputeFactorial(a),
        };

        private static Number ComputeFactorial(Number num)
        {
            // TODO: cache known factorials
            return num == 1m ? 1m : num * ComputeFactorial(num - 1);
        }

        public static Number Evaluate(string expression)
        {
            SourceText text = new SourceText(expression);
            Lexer lex = new Lexer(text);
            Parser parse = new Parser(lex);

            Expression expr = Reduce(parse.Parse());
            return ((NumberExpression)expr).Value;
        }

        internal static Expression Reduce(Expression expression)
        {
            if (expression is NumberExpression)
            {
                return expression;
            }

            var expr = _evaluators[expression.GetType()](expression);

            return Reduce(expr);
        }

        private static Expression EvaluateConstantExpression(Expression arg)
        {
            var constant = (ConstantExpression)arg;

            return new NumberExpression(Number.Parse(constant.Value));
        }

        private static Expression EvaluateGroupingExpression(Expression arg)
        {
            var grouping = (GroupingExpression)arg;
            return grouping.Inner;
        }

        private static Expression EvaluateUnaryExpression(Expression arg)
        {
            var unary = (UnaryExpression)arg;
            var value = (NumberExpression)Reduce(unary.Operand);

            var result = _unaryEvaluators[unary.OperatorType](value.Value);

            return new NumberExpression(result);
        }

        private static Expression EvaluateBinaryExpression(Expression expr)
        {
            var binary = (BinaryExpression)expr;
            var left = (NumberExpression)Reduce(binary.Left);
            var right = (NumberExpression)Reduce(binary.Right);

            var result = _binaryEvaluators[binary.OperatorType](left.Value, right.Value);
            return new NumberExpression(result);
        }
    }
}
