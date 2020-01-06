namespace StringMath
{
    internal class ConstantExpression : Expression
    {
        public string Value { get; }

        public ConstantExpression(string value)
        {
            Value = value;
        }
    }
}