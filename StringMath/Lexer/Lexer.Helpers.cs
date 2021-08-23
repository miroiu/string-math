using System.Text;

namespace StringMath
{
    internal sealed partial class Lexer
    {
        private string ReadIdentifier(ISourceText stream)
        {
            StringBuilder builder = new StringBuilder(12);
            stream.MoveNext();

            while (stream.Current != '}')
            {
                if (!char.IsLetter(stream.Current))
                {
                    throw new LangException($"Expected '}}' but found '{stream.Current}'");
                }

                builder.Append(stream.Current);
                stream.MoveNext();
            }

            stream.MoveNext();
            string text = builder.ToString();

            return string.IsNullOrWhiteSpace(text) ? throw new LangException($"Variable name cannot be empty.") : text;
        }

        private string ReadOperatorName(ISourceText stream)
        {
            StringBuilder builder = new StringBuilder(8);

            while (char.IsLetter(stream.Current))
            {
                builder.Append(stream.Current);
                stream.MoveNext();
            }

            string text = builder.ToString();
            return text;
        }

        private string ReadOperator(ISourceText stream)
        {
            StringBuilder builder = new StringBuilder(3);

            while (!_invalidOperatorCharacters.Contains(stream.Current))
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
                        throw new LangException($"Invalid number format: {builder}.");
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

            return stream.Peek(-1) == '.' ? throw new LangException($"Invalid number format: {builder}.") : builder.ToString();
        }

        private void ReadWhiteSpace(ISourceText stream)
        {
            while (char.IsWhiteSpace(stream.Current) && stream.MoveNext()) ;
        }
    }
}
