using System.Collections.Generic;

namespace StringMath
{
    /// <summary>A collection of variables.</summary>
    public interface IVariablesCollection : IEnumerable<string>
    {
        /// <summary>Overwrites the value of a variable.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The new value.</param>
        void SetValue(string name, double value);

        /// <summary>Gets the value of the variable.</summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The value of the variable.</param>
        /// <returns><c>true</c> if the variable exists, false otherwise.</returns>
        bool TryGetValue(string name, out double value);

        /// <inheritdoc cref="SetValue(string, double)" />
        double this[string name] { set; }

        /// <summary>Copies the variables to another collection.</summary>
        /// <param name="other">The other collection.</param>
        void CopyTo(IVariablesCollection other);
    }

    /// <inheritdoc />
    public sealed class VariablesCollection : Dictionary<string, double>, IVariablesCollection
    {
        /// <inheritdoc />
        public void CopyTo(IVariablesCollection other)
        {
            other.EnsureNotNull(nameof(other));

            foreach (var kvp in this)
            {
                other.SetValue(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc />
        public void SetValue(string name, double value)
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
