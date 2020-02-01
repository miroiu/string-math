using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StringMath.Tests")]
namespace StringMath
{
    public static class SMath
    {
        private static MathContext MathContext { get; } = new MathContext();
        private static Reducer Reducer { get; } = new Reducer();

        /// <summary>
        /// Add a new binary operator or overwrite an existing operator's logic. 
        /// </summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        /// <param name="precedence">Logarithmic precedence by default.</param>
        public static void AddOperator(string operatorName, Func<decimal, decimal, decimal> operation, Precedence precedence = default)
            => MathContext.AddBinaryOperator(operatorName, operation, precedence);

        /// <summary>
        /// Add a new unary operator or overwrite an existing operator's logic. 
        /// <see cref="Precedence"/> is always <see cref="Precedence.Prefix" />
        /// </summary>
        /// <param name="operatorName">The operator's string representation.</param>
        /// <param name="operation">The operation to execute for this operator.</param>
        public static void AddOperator(string operatorName, Func<decimal, decimal> operation)
            => MathContext.AddUnaryOperator(operatorName, operation);

        /// <summary>
        /// Evaluates a mathematical expression and returns a decimal value.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <returns>The result as a decimal value.</returns>
        public static decimal Evaluate(string expression)
            => Evaluate(expression, new Replacements());

        /// <summary>
        /// Evaluates a mathematical expression that contains variables and returns a decimal value.
        /// </summary>
        /// <param name="expression">The math expression to evaluate.</param>
        /// <param name="replacements">The variables to be replaced with their values.</param>
        /// <returns>The result as a decimal value.</returns>
        public static decimal Evaluate(string expression, Replacements replacements)
        {
            SourceText text = new SourceText(expression);
            Lexer lex = new Lexer(text, MathContext);
            Parser parse = new Parser(lex, MathContext);

            return Reducer.Reduce<ResultExpression>(parse.Parse(), MathContext, replacements).Value;
        }
    }
}
