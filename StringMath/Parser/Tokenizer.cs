﻿using System.Collections.Generic;
using System.Text;

namespace StringMath
{
    /// <inheritdoc />
    internal sealed partial class Tokenizer
    {
        private readonly SourceText _text;
        private readonly IMathContext _context;

        // Excluded characters for custom operators
        private static readonly HashSet<char> _invalidOperatorCharacters = new HashSet<char>
        {
            '(', ')', '{', '}', '!', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '\0'
        };

        /// <summary>Creates a new instance of the tokenizer.</summary>
        /// <param name="text">The text to tokenize.</param>
        /// <param name="context">The math context.</param>
        public Tokenizer(SourceText text, IMathContext context)
        {
            _text = text;
            _context = context;
        }

        /// <summary>Creates a new instance of the tokenizer.</summary>
        /// <param name="text">The text to tokenize.</param>
        /// <param name="context">The math context.</param>
        public Tokenizer(string text, IMathContext context) : this(new SourceText(text), context)
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

        private string ReadIdentifier(SourceText stream)
        {
            const char identifierTerminator = '}';

            StringBuilder builder = new StringBuilder(12);
            stream.MoveNext();

            if (char.IsLetter(stream.Current) || stream.Current == '_')
            {
                builder.Append(stream.Current);
                stream.MoveNext();
            }
            else
            {
                Token token = new Token(TokenType.Unknown, stream.Current.ToString(), stream.Position);
                throw MathException.UnexpectedToken(token, TokenType.Identifier);
            }

            while (stream.Current != identifierTerminator)
            {
                if (char.IsLetterOrDigit(stream.Current) || stream.Current == '_')
                {
                    builder.Append(stream.Current);
                    stream.MoveNext();
                }
                else
                {
                    Token token = new Token(TokenType.Unknown, stream.Current.ToString(), stream.Position);
                    throw MathException.UnexpectedToken(token, identifierTerminator);
                }
            }

            stream.MoveNext();
            string text = builder.ToString();

            if (text.Length == 0)
            {
                Token token = new Token(TokenType.Unknown, identifierTerminator.ToString(), stream.Position - 1);
                throw MathException.UnexpectedToken(token, identifierTerminator);
            }

            return text;
        }

        private string ReadOperator(SourceText stream)
        {
            StringBuilder builder = new StringBuilder(3);

            while (!char.IsWhiteSpace(stream.Current) && !_invalidOperatorCharacters.Contains(stream.Current))
            {
                builder.Append(stream.Current);
                stream.MoveNext();
            }

            string op = builder.ToString();
            if (IsOperator(op))
            {
                return builder.ToString();
            }

            for (int i = 0; i < op.Length; i++)
            {
                var possibleOperator = builder.ToString(0, i);
                if (IsOperator(possibleOperator))
                {
                    stream.Position -= op.Length - i;
                    return possibleOperator;
                }
            }

            return builder.ToString();
        }

        private bool IsOperator(string text)
        {
            return _context.IsBinary(text) || _context.IsUnary(text);
        }

        private string ReadNumber(SourceText stream)
        {
            StringBuilder builder = new StringBuilder(8);
            bool hasDot = false;

            while (true)
            {
                if (stream.Current == '.')
                {
                    if (!hasDot)
                    {
                        hasDot = true;

                        builder.Append(stream.Current);
                        stream.MoveNext();
                    }
                    else
                    {
                        Token token = new Token(TokenType.Unknown, stream.Current.ToString(), stream.Position);
                        throw MathException.UnexpectedToken(token, TokenType.Number);
                    }
                }
                else if (char.IsDigit(stream.Current))
                {
                    builder.Append(stream.Current);
                    stream.MoveNext();
                }
                else
                {
                    break;
                }
            }

            char peeked = stream.Peek(-1);

            if (peeked == '.')
            {
                Token token = new Token(TokenType.Unknown, peeked.ToString(), stream.Position);
                throw MathException.UnexpectedToken(token, TokenType.Number);
            }

            return builder.ToString();
        }

        private void ReadWhiteSpace(SourceText stream)
        {
            while (char.IsWhiteSpace(stream.Current) && stream.MoveNext()) { }
        }
    }
}
