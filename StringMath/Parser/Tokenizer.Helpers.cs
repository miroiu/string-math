using System.Text;

namespace StringMath
{
    internal sealed partial class Tokenizer
    {
        private string ReadIdentifier(ISourceText stream)
        {
            const char identifierTerminator = '}';

            StringBuilder builder = new StringBuilder("{", 12);
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

            builder.Append(stream.Current);
            stream.MoveNext();
            string text = builder.ToString();

            if (text.Length == 2)
            {
                Token token = new Token(TokenType.Unknown, identifierTerminator.ToString(), stream.Position - 1);
                throw MathException.UnexpectedToken(token, identifierTerminator);
            }

            return text;
        }

        private string ReadOperator(ISourceText stream)
        {
            StringBuilder builder = new StringBuilder(3);

            while (!char.IsWhiteSpace(stream.Current) && !_invalidOperatorCharacters.Contains(stream.Current))
            {
                builder.Append(stream.Current);
                stream.MoveNext();
            }

            return builder.ToString();
        }

        private string ReadNumber(ISourceText stream)
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

        private void ReadWhiteSpace(ISourceText stream)
        {
            while (char.IsWhiteSpace(stream.Current) && stream.MoveNext()) { }
        }
    }
}
