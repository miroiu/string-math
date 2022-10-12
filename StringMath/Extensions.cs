using StringMath.Expressions;
using System;

namespace StringMath
{
    /// <summary>Helpful extension methods.</summary>
    internal static class Extensions
    {
        /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="value"/> is null.</summary>
        /// <typeparam name="T">The parameter's type.</typeparam>
        /// <param name="value">The parameter's value.</param>
        /// <param name="name">The parameter's name.</param>
        public static void EnsureNotNull<T>(this T value, string name) where T : class
        {
            if (value == default)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>Converts a token type to a readable string.</summary>
        /// <param name="tokenType">The token type.</param>
        /// <returns>A readable string.</returns>
        public static string ToReadableString(this TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.Identifier => "identifier",
                TokenType.Number => "number",
                TokenType.Operator => "operator",
                TokenType.EndOfCode => "[EOC]",
                TokenType.OpenParen => "(",
                TokenType.CloseParen => ")",
                TokenType.Exclamation => "!",
                _ => tokenType.ToString(),
            };
        }

        public static IExpression Parse(this string text, IMathContext context)
        {
            text.EnsureNotNull(nameof(text));

            ITokenizer tokenizer = new Tokenizer(text);
            IParser parser = new Parser(tokenizer, context);
            return parser.Parse();
        }
    }

    /// <summary>Extensions for <see cref="MathExpr"/>.</summary>
    public static class MathExprExtensions
    {
        public static MathExpr ToMathExpr(this string expr) => (MathExpr)expr;
        public static MathExpr ToMathExpr(this string expr, IMathContext context) => new MathExpr(expr, context);

        public static double Eval(this string value) => value.ToMathExpr().Result;

        public static double Eval(this string value, IMathContext context) => value.ToMathExpr(context).Result;

        public static MathExpr Substitute(this string expr, string var, double val) => expr.ToMathExpr().Substitute(var, val);
    }
}
