using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace StringMath.Tests
{
    [TestFixture]
    internal class Tests
    {
        public static Token[] GetTokens(string input)
        {
            var sourceText = new SourceText(input);
            var lexer = new Lexer(sourceText);

            var tokens = new List<Token>();
            Token t = lexer.Lex();
            tokens.Add(t);

            while (t.Type != TokenType.EndOfCode)
            {
                t = lexer.Lex();
                tokens.Add(t);
            }

            return tokens.RemoveLast();
        }

        [Test]
        [TestCase("1", new[] { TokenType.Number })]
        [TestCase("1+2", new[] { TokenType.Number, TokenType.Operator, TokenType.Number })]
        [TestCase("-1 * 3.5", new[] { TokenType.Operator, TokenType.Number, TokenType.Operator, TokenType.Number })]
        public void TestCorrectSyntax(string input, TokenType[] expected)
        {
            var source = new SourceText(input);
            var lexer = new Lexer(source);

            Assert.AreEqual(expected, GetTokens(input).Select(t => t.Type).ToArray());
        }
    }
}