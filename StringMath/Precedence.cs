using System;

namespace StringMath
{
    public sealed class Precedence : IEquatable<Precedence>
    {
        public static readonly Precedence None = new Precedence(int.MinValue);
        public static readonly Precedence Addition = new Precedence(0);
        public static readonly Precedence Multiplication = new Precedence(1);
        public static readonly Precedence Power = new Precedence(2);
        public static readonly Precedence Logarithmic = new Precedence(3);
        public static readonly Precedence UserDefined = new Precedence(4);
        public static readonly Precedence Prefix = new Precedence(int.MaxValue);

        private readonly int _value;

        private Precedence(int value)
            => _value = value;

        public static implicit operator int(Precedence precedence)
            => precedence?._value ?? None;

        public static implicit operator Precedence(int precedence)
            => new Precedence(precedence);

        public bool Equals(Precedence other)
        {
            return other != default && other._value == _value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Precedence);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}