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
        OpenCurly,
        CloseCurly,

        // Operators
        Operator,
        Exclamation
    }
}