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

        public static implicit operator Number(decimal num)
            => new Number(num);

        public static Number operator +(Number a, Number b)
            => a.Value + b.Value;
        public static Number operator -(Number a, Number b)
            => a.Value - b.Value;
        public static Number operator /(Number a, Number b)
            => a.Value / b.Value;
        public static Number operator *(Number a, Number b)
            => a.Value * b.Value;

        public static Number operator -(Number a)
            => -a.Value;

        public static bool operator ==(Number a, decimal b)
            => a.Value == b;

        public static bool operator !=(Number a, decimal b)
            => a.Value != b;
    }
}