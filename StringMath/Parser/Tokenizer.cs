using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StringMath
{
#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER

    /// <inheritdoc />
    internal sealed class Tokenizer
    {
        private readonly List<Token> _tokens = new List<Token>(8);
        private readonly int _length;
        private int _currentIndex;
        private int _tokenIndex;

        // Excluded characters for custom operators
        private static readonly HashSet<char> _invalidOperatorCharacters = new HashSet<char>
        {
            '(', ')', '{', '}', '!', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '\0'
        };

        /// <summary>Creates a new instance of the tokenizer.</summary>
        /// <param name="text">The text to tokenize.</param>
        public Tokenizer(string text) : this(text.AsSpan())
        {
        }

        private bool AtEnd
        {
            get
            {
                Debug.Assert(_currentIndex <= _length);
                return _currentIndex >= _length;
            }
        }

        public Tokenizer(ReadOnlySpan<char> text)
        {
            _length = text.Length;

            while (!AtEnd)
            {
                switch (text[_currentIndex])
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
                        _tokens.Add(ReadNumber(ref text));
                        break;

                    case '(':
                        MoveNext();
                        _tokens.Add(new Token(TokenType.OpenParen, "(", _currentIndex));
                        break;

                    case ')':
                        MoveNext();
                        _tokens.Add(new Token(TokenType.CloseParen, ")", _currentIndex));
                        break;

                    case '{':
                        _tokens.Add(ReadIdentifier(ref text));
                        break;

                    case '!':
                        MoveNext();
                        _tokens.Add(new Token(TokenType.Exclamation, "!", _currentIndex));
                        break;

                    case ' ':
                    case '\t':
                        SkipWhiteSpace(ref text);
                        break;

                    case '\r':
                    case '\n':
                        MoveNext();
                        break;

                    case '\0':
                        _tokens.Add(new Token(TokenType.EndOfCode, "\0", _currentIndex));
                        MoveNext();
                        break;

                    default:
                        _tokens.Add(ReadOperator(ref text));
                        break;
                }
            }

            var endOfCodeToken = new Token(TokenType.EndOfCode, "\0", _currentIndex);
            _tokens.Add(endOfCodeToken);
        }

        private void MoveNext()
        {
            Debug.Assert(_currentIndex < _length);
            _currentIndex++;
        }

        /// <inheritdoc />
        public Token ReadToken()
        {
            if (_tokenIndex < _tokens.Count)
            {
                return _tokens[_tokenIndex++];
            }

            //Debug.Fail("Reading more tokens than expected.");
            return _tokens[^1];
        }

        private Token ReadIdentifier(ref ReadOnlySpan<char> input)
        {
            const char identifierTerminator = '}';
            var endIndex = input[_currentIndex..].IndexOf(identifierTerminator) + _currentIndex;

            if (endIndex - _currentIndex < 2)
            {
                var unexpectedToken = new Token(TokenType.Unknown, input[_currentIndex].ToString(), _currentIndex);
                throw MathException.UnexpectedToken(unexpectedToken, TokenType.Identifier);
            }

            var identifier = input.Slice(_currentIndex + 1, endIndex - _currentIndex - 1);

            if (identifier.Length == 0)
            {
                var unexpectedToken = new Token(TokenType.Unknown, identifierTerminator.ToString(), _currentIndex);
                throw MathException.UnexpectedToken(unexpectedToken, identifierTerminator);
            }

            // Make sure we start with a letter or an underscore
            if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
            {
                var unexpectedToken = new Token(TokenType.Unknown, identifier[0].ToString(), _currentIndex);
                throw MathException.UnexpectedToken(unexpectedToken, TokenType.Identifier);
            }

            for (int i = 0; i < identifier.Length; i++)
            {
                if (!char.IsLetterOrDigit(identifier[i]) && identifier[i] != '_')
                {
                    var unexpectedToken = new Token(TokenType.Unknown, identifier[i].ToString(), _currentIndex + i);
                    throw MathException.UnexpectedToken(unexpectedToken, identifierTerminator);
                }
            }

            _currentIndex += identifier.Length + 2;

            var token = new Token(TokenType.Identifier, identifier.ToString(), _currentIndex);
            return token;
        }

        private Token ReadOperator(ref ReadOnlySpan<char> input)
        {
            if (input.Length > 0 && _invalidOperatorCharacters.Contains(input[_currentIndex]))
            {
                var unexpectedToken = new Token(TokenType.Unknown, input[_currentIndex].ToString(), _currentIndex);
                throw MathException.UnexpectedToken(unexpectedToken, TokenType.Operator);
            }

            int startIndex = _currentIndex;
            do
            {
                MoveNext();
            }
            while (!AtEnd && !char.IsWhiteSpace(input[_currentIndex]) && !_invalidOperatorCharacters.Contains(input[_currentIndex]));

            var token = new Token(TokenType.Operator, input[startIndex.._currentIndex].ToString(), _currentIndex);
            return token;
        }

        private Token ReadNumber(ref ReadOnlySpan<char> input)
        {
            int startIndex = _currentIndex;
            bool hasDot = false;

            do
            {
                var currentChar = input[_currentIndex];
                if (currentChar == '.')
                {
                    if (!hasDot)
                    {
                        hasDot = true;
                        MoveNext();
                    }
                    else
                    {
                        var unexpectedToken = new Token(TokenType.Unknown, currentChar.ToString(), _currentIndex);
                        throw MathException.UnexpectedToken(unexpectedToken, TokenType.Number);
                    }
                }
                else if (char.IsDigit(currentChar))
                {
                    MoveNext();
                }
                else
                {
                    break;
                }
            }
            while (!AtEnd);

            if (_currentIndex >= 1 && _currentIndex <= _length && !char.IsDigit(input[_currentIndex - 1]))
            {
                var unexpectedToken = new Token(TokenType.Unknown, input[_currentIndex - 1].ToString(), _currentIndex - 1);
                throw MathException.UnexpectedToken(unexpectedToken, TokenType.Number);
            }

            var token = new Token(TokenType.Number, input[startIndex.._currentIndex].ToString(), _currentIndex);
            return token;
        }

        private void SkipWhiteSpace(ref ReadOnlySpan<char> input)
        {
            while (!AtEnd && char.IsWhiteSpace(input[_currentIndex]))
            {
                MoveNext();
            }
        }
    }

#endif
}
