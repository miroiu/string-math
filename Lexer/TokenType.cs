namespace SCLang
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
        Plus,
        Minus,
        Asterisk,
        Slash,
        Exclamation
    }
}