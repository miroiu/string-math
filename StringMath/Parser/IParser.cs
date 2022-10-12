using StringMath.Expressions;

namespace StringMath
{
    /// <summary>Contract for parsers.</summary>
    internal interface IParser
    {
        /// <summary>Creates an expression tree from a token stream.</summary>
        /// <returns>The resulting expression tree.</returns>
        IExpression Parse();
    }
}