namespace StringMath
{
    public class Number
    {
        public Number(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }

        public static Number Parse(string value)
            => new Number(decimal.Parse(value));

        public static explicit operator decimal(Number num)
            => num.Value;
    }
}