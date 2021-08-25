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

        /// <summary>Multiplication precedence (1).</summary>
        public static readonly Precedence Multiplication = new Precedence(1);

        /// <summary>Power precedence (2).</summary>
        public static readonly Precedence Power = new Precedence(2);

        /// <summary>Logarithmic precedence (3).</summary>
        public static readonly Precedence Logarithmic = new Precedence(3);

        /// <summary>User defined precedence (4).</summary>
        public static readonly Precedence UserDefined = new Precedence(4);

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
        {
            return other._value == _value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Precedence && Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <inheritdoc />
        public static bool operator ==(Precedence left, Precedence right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(Precedence left, Precedence right)
        {
            return !(left == right);
        }
    }
}