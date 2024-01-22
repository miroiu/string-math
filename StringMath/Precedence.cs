using System;

namespace StringMath
{
    /// <summary>The operator precedence.</summary>
    public readonly struct Precedence : IEquatable<Precedence>
    {
        /// <summary>The lowest precedence value.</summary>
        public static readonly Precedence None = new Precedence(int.MinValue);

        /// <summary>Addition precedence (0).</summary>
        public static readonly Precedence Addition = new Precedence(0);

        /// <summary>Multiplication precedence (4).</summary>
        public static readonly Precedence Multiplication = new Precedence(4);

        /// <summary>Power precedence (8).</summary>
        public static readonly Precedence Power = new Precedence(8);

        /// <summary>Logarithmic precedence (16).</summary>
        public static readonly Precedence Logarithmic = new Precedence(16);

        /// <summary>User defined precedence (32).</summary>
        public static readonly Precedence UserDefined = new Precedence(32);

        /// <summary>The highest precedence value.</summary>
        public static readonly Precedence Prefix = new Precedence(int.MaxValue);

        private readonly int _value;

        private Precedence(int value)
            => _value = value;

        /// <summary>Gets the value of precedence.</summary>
        /// <param name="precedence">The precedence.</param>
        public static implicit operator int(Precedence? precedence)
            => precedence?._value ?? None;

        /// <summary>Gets the precedence from a value.</summary>
        /// <param name="precedence">The value.</param>
        public static implicit operator Precedence(int precedence)
        {
            return precedence switch
            {
                int.MinValue => None,
                0 => Addition,
                1 => Multiplication,
                2 => Power,
                3 => Logarithmic,
                4 => UserDefined,
                5 => Prefix,
                _ => new Precedence(precedence),
            };
        }

        /// <inheritdoc />
        public bool Equals(Precedence other)
            => other._value == _value;

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Precedence && Equals(obj);

        /// <inheritdoc />
        public override int GetHashCode()
            => _value.GetHashCode();

        /// <inheritdoc />
        public static bool operator ==(Precedence left, Precedence right)
            => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(Precedence left, Precedence right)
            => !(left == right);
    }
}