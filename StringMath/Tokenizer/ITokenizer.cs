namespace StringMath
{
    /// <summary>Contract for tokenizers.</summary>
    internal interface ITokenizer
    {
        /// <summary>Reads the next token in the token stream.</summary>
        /// <returns>A token.</returns>
        Token ReadToken();
    }
}
