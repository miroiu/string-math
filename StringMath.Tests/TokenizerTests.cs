using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace StringMath.Tests
{
    [TestFixture]
    internal class TokenizerTests
    {
        [Test]
        [TestCase("-1 * 3.5", new[] { TokenType.Operator, TokenType.Number, TokenType.Operator, TokenType.Number })]
        [TestCase("2 pow 3", new[] { TokenType.Number, TokenType.Operator, TokenType.Number })]
        [TestCase("{a} + 2", new[] { TokenType.Identifier, TokenType.Operator, TokenType.Number })]
        [TestCase("(-1) + 2", new[] { TokenType.OpenParen, TokenType.Operator, TokenType.Number, TokenType.CloseParen, TokenType.Operator, TokenType.Number })]
        [TestCase("5!", new[] { TokenType.Number, TokenType.Exclamation })]
        public void ReadToken(string input, TokenType[] expected)
        {
            IEnumerable<TokenType> expectedTokens = expected.Append(TokenType.EndOfCode);
            IEnumerable<TokenType> actualTokens = input.ReadAllTokens().Select(t => t.Type);
            Assert.That(actualTokens, Is.EquivalentTo(expectedTokens));
        }

        [Test]
        [TestCase("  123 1")]
        [TestCase("123\n2")]
        [TestCase("\t123\n  3")]
        [TestCase("\t123\r\n 5")]
        public void ReadToken_IgnoresWhitespace(string input)
        {
            // Arrange
            ITokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token1 = tokenizer.ReadToken();
            Token token2 = tokenizer.ReadToken();
            Token token3 = tokenizer.ReadToken();

            // Assert
            Assert.AreEqual(TokenType.Number, token1.Type);
            Assert.AreEqual(TokenType.Number, token2.Type);
            Assert.AreEqual(TokenType.EndOfCode, token3.Type);
        }

        [Test]
        [TestCase("{a}")]
        [TestCase("{asdas}")]
        [TestCase("{x1}")]
        [TestCase("{x_1}")]
        [TestCase("{x_}")]
        [TestCase("{_x}")]
        [TestCase("{_}")]
        public void ReadIdentifier(string input)
        {
            // Arrange
            ITokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token = tokenizer.ReadToken();

            // Assert
            Assert.AreEqual(TokenType.Identifier, token.Type);
            Assert.AreEqual(input, token.Text);
        }

        [Test]
        [TestCase("{}")]
        [TestCase("{ }")]
        [TestCase("{  }")]
        [TestCase("{1}")]
        [TestCase("{a.}")]
        [TestCase("{1a}")]
        [TestCase("{{a}")]
        [TestCase("{a a}")]
        public void ReadIdentifier_Exception(string input)
        {
            // Arrange
            ITokenizer tokenizer = new Tokenizer(input);

            // Act & Assert
            LangException exception = Assert.Throws<LangException>(() => tokenizer.ReadToken());
            Assert.AreEqual(LangException.ErrorCode.UNEXPECTED_TOKEN, exception.ErrorType);
        }

        [Test]
        [TestCase("pow")]
        [TestCase("<")]
        [TestCase("**")]
        [TestCase("<a>")]
        [TestCase("o&")]
        [TestCase("a@a")]
        public void ReadOperator(string input)
        {
            // Arrange
            ITokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token = tokenizer.ReadToken();

            // Assert
            Assert.AreEqual(TokenType.Operator, token.Type);
            Assert.AreEqual(input, token.Text);
        }

        [Test]
        [TestCase(".1")]
        [TestCase("0.1")]
        [TestCase("0.00000000002")]
        [TestCase("0")]
        [TestCase("9999")]
        [TestCase("9999.0")]
        [TestCase("99.01")]
        public void ReadNumber(string input)
        {
            // Arrange
            ITokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token = tokenizer.ReadToken();

            // Assert
            Assert.AreEqual(TokenType.Number, token.Type);
            Assert.AreEqual(input, token.Text);
        }

        [Test]
        [TestCase("1.")]
        [TestCase("..1")]
        [TestCase("1..")]
        [TestCase("1.0.")]
        [TestCase("1.0.1")]
        [TestCase(".a")]
        public void ReadNumber_Exception(string input)
        {
            // Arrange
            ITokenizer tokenizer = new Tokenizer(input);

            // Act & Assert
            LangException exception = Assert.Throws<LangException>(() => tokenizer.ReadToken());
            Assert.AreEqual(LangException.ErrorCode.UNEXPECTED_TOKEN, exception.ErrorType);
        }
    }
}