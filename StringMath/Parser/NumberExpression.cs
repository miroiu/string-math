namespace StringMath
{
    internal class NumberExpression : Expression
    {
        public string Value { get; }

        public NumberExpression(string value)
        {
            Value = value;
        }
    }
}