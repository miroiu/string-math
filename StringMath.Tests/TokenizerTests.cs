using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace StringMath.Tests
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("-1 * 3.5", new[] { TokenType.Operator, TokenType.Number, TokenType.Operator, TokenType.Number })]
        [InlineData("2 pow 3", new[] { TokenType.Number, TokenType.Operator, TokenType.Number })]
        [InlineData("{a} + 2", new[] { TokenType.Identifier, TokenType.Operator, TokenType.Number })]
        [InlineData("(-1) + 2", new[] { TokenType.OpenParen, TokenType.Operator, TokenType.Number, TokenType.CloseParen, TokenType.Operator, TokenType.Number })]
        [InlineData("5!", new[] { TokenType.Number, TokenType.Exclamation })]
        internal void ReadToken(string input, TokenType[] expected)
        {
            IEnumerable<TokenType> actualTokens = input.ReadAllTokens()
                .Where(token => token.Type != TokenType.EndOfCode)
                .Select(t => t.Type);
            Assert.Equal(actualTokens, expected);
        }

        [Theory]
        [InlineData("  123 1")]
        [InlineData("123\n2")]
        [InlineData("\t123\n  3")]
        [InlineData("\t123\r\n 5")]
        public void ReadToken_IgnoresWhitespace(string input)
        {
            // Arrange
            Tokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token1 = tokenizer.ReadToken();
            Token token2 = tokenizer.ReadToken();
            Token token3 = tokenizer.ReadToken();

            // Assert
            Assert.Equal(TokenType.Number, token1.Type);
            Assert.Equal(TokenType.Number, token2.Type);
            Assert.Equal(TokenType.EndOfCode, token3.Type);
        }

        [Theory]
        [InlineData("{a}")]
        [InlineData("{asdas}")]
        [InlineData("{x1}")]
        [InlineData("{x_1}")]
        [InlineData("{x_x}")]
        [InlineData("{x_}")]
        [InlineData("{x1_}")]
        [InlineData("{_x}")]
        [InlineData("{_}")]
        [InlineData("{_1}")]
        [InlineData("{_1a}")]
        [InlineData("{_a1}")]
        public void ReadIdentifier(string input)
        {
            // Arrange
            Tokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token = tokenizer.ReadToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal(input, $"{{{token.Text}}}");
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{ }")]
        [InlineData("{  }")]
        [InlineData("{1}")]
        [InlineData("{a.}")]
        [InlineData("{1a}")]
        [InlineData("{{a}")]
        [InlineData("{a a}")]
        [InlineData("{-a}")]
        [InlineData("1 * {}")]
        [InlineData("{")]
        [InlineData("}")]
        [InlineData("{a")]
        public void ReadIdentifier_Exception(string input)
        {
            // Arrange & Act & Assert
            MathException exception = Assert.Throws<MathException>(() => new Tokenizer(input));
            Assert.Equal(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Theory]
        [InlineData("pow")]
        [InlineData("<")]
        [InlineData("**")]
        [InlineData("<a>")]
        [InlineData("o&")]
        [InlineData("a@a")]
        public void ReadOperator(string input)
        {
            // Arrange
            Tokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token = tokenizer.ReadToken();

            // Assert
            Assert.Equal(TokenType.Operator, token.Type);
            Assert.Equal(input, token.Text);
        }

        [Theory]
        [InlineData(".1")]
        [InlineData("0.1")]
        [InlineData("0.00000000002")]
        [InlineData("0")]
        [InlineData("9999")]
        [InlineData("9999.0")]
        [InlineData("99.01")]
        public void ReadNumber(string input)
        {
            // Arrange
            Tokenizer tokenizer = new Tokenizer(input);

            // Act
            Token token = tokenizer.ReadToken();

            // Assert
            Assert.Equal(TokenType.Number, token.Type);
            Assert.Equal(input, token.Text);
        }

        [Theory]
        [InlineData("1.")]
        [InlineData("..1")]
        [InlineData("1..")]
        [InlineData("1.0.")]
        [InlineData("1.0.1")]
        [InlineData(".a")]
        [InlineData("1..1")]
        [InlineData("1.a")]
        public void ReadNumber_Exception(string input)
        {
            // Arrange & Act & Assert
            MathException exception = Assert.Throws<MathException>(() => new Tokenizer(input));
            Assert.Equal(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }
    }
}