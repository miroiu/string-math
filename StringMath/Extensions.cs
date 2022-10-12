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
        /// <summary>Converts a string expression to a <see cref="MathExpr"/>.</summary>
        /// <param name="expr">The string to convert.</param>
        /// <returns>A <see cref="MathExpr"/>.</returns>
        public static MathExpr ToMathExpr(this string expr) => (MathExpr)expr;

        /// <summary>Converts a string expression to a <see cref="MathExpr"/>.</summary>
        /// <param name="expr">The string to convert.</param>
        /// <param name="context">The context to use for the resulting expression.</param>
        /// <returns>A <see cref="MathExpr"/>.</returns>
        public static MathExpr ToMathExpr(this string expr, IMathContext context) => new MathExpr(expr, context);

        /// <summary>Evaluates a math expression from a string.</summary>
        /// <param name="value">The math expression.</param>
        /// <returns>The result as a double.</returns>
        public static double Eval(this string value) => value.ToMathExpr().Result;

        /// <summary>Evaluates a math expression from a string in a given context.</summary>
        /// <param name="value">The math expression.</param>
        /// <param name="context">The context used to evaluate the expression.</param>
        /// <returns>The result as a double.</returns>
        public static double Eval(this string value, IMathContext context) => value.ToMathExpr(context).Result;

        /// <summary>Converts a string to a <see cref="MathExpr"/> and substitutes the given variable.</summary>
        /// <param name="expr">The math expression.</param>
        /// <param name="var">The variable's name.</param>
        /// <param name="val">The variable's value.</param>
        /// <returns>A <see cref="MathExpr"/>.</returns>
        public static MathExpr Substitute(this string expr, string var, double val) => expr.ToMathExpr().Substitute(var, val);
    }
}
