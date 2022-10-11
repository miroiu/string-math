using StringMath.Expressions;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>Contract for parsers.</summary>
    internal interface IParser
    {
        /// <summary>A collection of variables extracted from an expression tree.</summary>
        IReadOnlyCollection<string> Variables { get; }

        /// <summary>Creates an expression tree from a token stream.</summary>
        /// <returns>The resulting expression tree.</returns>
        IExpression Parse();
    }
}