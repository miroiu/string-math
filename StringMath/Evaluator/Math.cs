using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StringMath.Tests")]
namespace StringMath
{
    public static class Math
    {
        private static readonly Dictionary<Type, Func<IMathContext, Expression>> _evaluators = new Dictionary<Type, Func<IMathContext, Expression>>
        {

        };

        public static Number Evaluate(string expression)
        {
            SourceText text = new SourceText(expression);
            Lexer lex = new Lexer(text);
            Parser parse = new Parser(lex);

            Expression expr = Reduce(parse.Parse(), default);
            return Number.Parse(((NumberExpression)expr).Value);
        }

        internal static Expression Reduce(Expression expression, IMathContext ctx)
        {
            if (expression is NumberExpression)
            {
                return expression;
            }

            var expr = _evaluators[expression.GetType()](ctx);

            return Reduce(expr, ctx);
        }
    }
}
