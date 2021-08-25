using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    internal sealed partial class Tokenizer : ITokenizer
    {
        private readonly ISourceText _text;

        // Excluded characters for custom operators
        private readonly HashSet<char> _invalidOperatorCharacters = new HashSet<char>
        {
            '(', ')', '{', '}', '!', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '\0'
        };

        /// <summary>Creates a new instance of the tokenizer.</summary>
        /// <param name="text">The text to tokenize.</param>
        public Tokenizer(ISourceText text)
        {
            _text = text;
        }

        /// <summary>Creates a new instance of the tokenizer.</summary>
        /// <param name="text">The text to tokenize.</param>
        public Tokenizer(string text) : this(new SourceText(text))
        {
        }

        /// <inheritdoc />
        public Token ReadToken()
        {
            switch (_text.Current)
            {
                case '.':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return new Token(TokenType.Number, ReadNumber(_text), _text.Position);

                case '(':
                    _text.MoveNext();
                    return new Token(TokenType.OpenParen, "(", _text.Position);

                case ')':
                    _text.MoveNext();
                    return new Token(TokenType.CloseParen, ")", _text.Position);

                case '{':
                    return new Token(TokenType.Identifier, ReadIdentifier(_text), _text.Position);

                case '!':
                    _text.MoveNext();
                    return new Token(TokenType.Exclamation, "!", _text.Position);

                case ' ':
                case '\t':
                    ReadWhiteSpace(_text);
                    return ReadToken();

                case '\r':
                case '\n':
                    _text.MoveNext();
                    return ReadToken();

                case '\0':
                    return new Token(TokenType.EndOfCode, "\0", _text.Position);

                default:
                    string op = ReadOperator(_text);
                    return new Token(TokenType.Operator, op, _text.Position);
            }
        }

        /// <inheritdoc />
        public override string? ToString()
        {
            return _text.ToString();
        }
    }
}
