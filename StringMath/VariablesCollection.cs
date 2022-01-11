using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StringMath
{
    /// <summary>A collection of variables.</summary>
    public interface IVariablesCollection<TNum> : IEnumerable<string> where TNum : INumber<TNum>
    {
        /// <summary>Overwrites the value of a variable.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The new value.</param>
        void SetValue(string name, TNum value);

        /// <summary>Gets the value of the variable.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The value of the variable.</param>
        /// <returns><c>true</c> if the variable exists, false otherwise.</returns>
        bool TryGetValue(string name, [MaybeNullWhen(false)] out TNum value);

        /// <inheritdoc cref="SetValue(string, TNum)" />
        TNum this[string name] { set; }

        /// <summary>Copies the variables to another collection.</summary>
        /// <param name="other">The other collection.</param>
        void CopyTo(IVariablesCollection<TNum> other);
    }

    /// <inheritdoc />
    public class VariablesCollection<TNum> : Dictionary<string, TNum>, IVariablesCollection<TNum> where TNum : INumber<TNum>
    {
        /// <inheritdoc />
        public void CopyTo(IVariablesCollection<TNum> other)
        {
            other.EnsureNotNull(nameof(other));

            foreach (var kvp in this)
            {
                other.SetValue(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc />
        public void SetValue(string name, TNum value)
        {
            name.EnsureNotNull(nameof(name));
            base[name] = value;
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return Keys.GetEnumerator();
        }
    }
}
