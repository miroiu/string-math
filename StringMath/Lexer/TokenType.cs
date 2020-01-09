namespace StringMath
{
    internal enum TokenType
    {
        EndOfCode,

        Identifier,
        Number,

        // Separators
        OpenParen,
        CloseParen,

        // Operators
        Operator,
        Exclamation
    }
}