namespace StringMath
{
    /// <summary>A token containing basic information about some text.</summary>
    internal readonly struct Token
    {
        /// <summary>Initializes a new instance of a token.</summary>
        /// <param name="type">The token type.</param>
        /// <param name="text">The token value.</param>
        /// <param name="position">The token's position in the input string.</param>
        public Token(TokenType type, string text, int position)
        {
            Type = type;
            Text = text;
            Position = position;
        }

        /// <summary>The token's position in the input string.</summary>
        public readonly int Position;

        /// <summary>The token value.</summary>
        public readonly string Text;

        /// <summary>The token type.</summary>
        public readonly TokenType Type;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Text} ({Type}):{Position}";
        }
    }
}