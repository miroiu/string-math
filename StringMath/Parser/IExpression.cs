namespace StringMath
{
    /// <summary>Contract for expressions.</summary>
    internal interface IExpression
    {
        /// <summary>The type of the expression.</summary>
        ExpressionType Type { get; }
    }
}
