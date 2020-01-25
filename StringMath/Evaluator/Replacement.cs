namespace StringMath
{
    public class Replacement
    {
        public Replacement(string identifier, decimal value)
        {
            Identifier = identifier;
            Value = value;
        }

        public string Identifier { get; }
        public decimal Value { get; }
    }
}