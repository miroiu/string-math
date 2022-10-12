namespace StringMath
{
    /// <summary>Available token types.</summary>
    internal enum TokenType
    {
        /// <summary>Unknown token.</summary>
        Unknown,

        /// <summary>\0</summary>
        EndOfCode,

        /// <summary>[aA-zZ_]+[aA-zZ0-9_]</summary>
        Identifier,
        /// <summary>1 or .1 or 1.1</summary>
        Number,

        /// <summary>(</summary>
        OpenParen,
        /// <summary>)</summary>
        CloseParen,

        /// <summary>Everything excluding ( ) { } ! . 0 1 2 3 4 5 6 7 8 9 \0</summary>
        Operator,

        /// <summary>!</summary>
        Exclamation
    }
}