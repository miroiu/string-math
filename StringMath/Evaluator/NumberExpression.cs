namespace StringMath
{
    internal class NumberExpression : Expression
    {
        public NumberExpression(Number number)
        {
            Value = number;
        }

        public Number Value { get; }
    }
}
