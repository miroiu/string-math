using NUnit.Framework;
using System.Linq;

namespace StringMath.Tests
{
    [TestFixture]
    internal class Tests
    {
        private MathContext _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new MathContext();
            _context.AddBinaryOperator("pow", default);
            _context.AddBinaryOperator("<<<", default);
        }

        [Test]
        [TestCase("1", new[] { TokenType.Number })]
        [TestCase("1+2", new[] { TokenType.Number, TokenType.Operator, TokenType.Number })]
        [TestCase("-1 * 3.5", new[] { TokenType.Operator, TokenType.Number, TokenType.Operator, TokenType.Number })]
        [TestCase("2 pow 3", new[] { TokenType.Number, TokenType.Operator, TokenType.Number })]
        [TestCase("{a} + 2", new[] { TokenType.Identifier, TokenType.Operator, TokenType.Number })]
        [TestCase("(-1) + 2", new[] { TokenType.OpenParen, TokenType.Operator, TokenType.Number, TokenType.CloseParen, TokenType.Operator, TokenType.Number })]
        [TestCase("<<<", new[] { TokenType.Operator })]
        public void TestCorrectSyntax(string input, TokenType[] expected)
        {
            Assert.AreEqual(expected, input.GetTokens(_context).Select(t => t.Type).ToArray());
        }
    }
}