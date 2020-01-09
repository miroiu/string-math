using System.Text;

namespace StringMath
{
    internal partial class Lexer
    {
        private string ReadIdentifier(SourceText stream)
        {
            StringBuilder builder = new StringBuilder(12);
            stream.MoveNext();

            while (stream.Current != '}')
            {
                if (!char.IsLetter(stream.Current))
                {
                    throw new LangException($"Expected '}}' but found {stream.Current}");
                }

                builder.Append(stream.Current);
                stream.MoveNext();
            }

            var text = builder.ToString();
            stream.MoveNext();
            return text;
        }

        private string ReadOperatorName(SourceText stream)
        {
            StringBuilder builder = new StringBuilder(8);

            while (char.IsLetter(stream.Current))
            {
                builder.Append(stream.Current);
                stream.MoveNext();
            }

            var text = builder.ToString();
            return text;
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
                        throw new LangException($"Invalid number format.");
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

            if (stream.Peek(-1) == '.')
            {
                throw new LangException($"Invalid number format.");
            }

            return builder.ToString();
        }

        private void ReadWhiteSpace(SourceText stream)
        {
            while (char.IsWhiteSpace(stream.Current) && stream.MoveNext()) ;
        }
    }
}
