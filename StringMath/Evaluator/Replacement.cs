namespace StringMath
{
    public class Replacement
    {
        public Replacement(string identifier, Number value)
        {
            Identifier = identifier;
            Value = value;
        }

        public string Identifier { get; }
        public Number Value { get; }
    }
}