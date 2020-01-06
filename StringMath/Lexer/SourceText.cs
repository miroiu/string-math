using System.Collections;
using System.Collections.Generic;

namespace StringMath
{
    internal class SourceText : IEnumerator<char>
    {
        public string Text { get; }

        public int Position { get; private set; }
        public int Length => Text.Length;

        // The string terminator is used by the lexer to produce EndOfCode tokens
        public SourceText(string source)
            => Text = $"{source}\0";

        public char Current => Text[Position];

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (Position + 1 < Text.Length)
            {
                Position++;
                return true;
            }

            return false;
        }

        public char Peek(int count = 1)
        {
            var location = Position + count;

            if (location < Text.Length && location >= 0)
            {
                return Text[location];
            }

            return '\0';
        }

        public void Reset()
            => Position = 0;

        public void Dispose()
            => Reset();
    }
}
