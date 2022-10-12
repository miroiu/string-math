using System;
using System.Collections;
using System.Collections.Generic;

namespace StringMath
{
    /// <summary>A collection of variables.</summary>
    internal interface IVariablesCollection : IEnumerable<string>
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

        /// <summary>Tells whether the variable is defined or not.</summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>True if variable was previously defined. False otherwise.</returns>
        bool Contains(string name);

        /// <inheritdoc cref="SetValue(string, double)" />
        double this[string name] { set; }
    }

    /// <inheritdoc />
    internal class VariablesCollection : IVariablesCollection
    {
        public static readonly VariablesCollection Default = new VariablesCollection();

        private readonly Dictionary<string, double> _values = new Dictionary<string, double>();

        static VariablesCollection()
        {
            Default["PI"] = Math.PI;
            Default["E"] = Math.E;
        }

        /// <inheritdoc />
        public void CopyTo(IVariablesCollection other)
        {
            other.EnsureNotNull(nameof(other));

            foreach (var kvp in _values)
            {
                other.SetValue(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator() => _values.GetEnumerator();

        /// <inheritdoc />
        public double this[string name]
        {
            set => SetValue(name, value);
        }

        /// <inheritdoc />
        public void SetValue(string name, double value)
        {
            name.EnsureNotNull(nameof(name));
            _values[name] = value;
        }

        /// <inheritdoc />
        public bool TryGetValue(string name, out double value)
            => _values.TryGetValue(name, out value) || Default._values.TryGetValue(name, out value);

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => _values.Keys.GetEnumerator();

        /// <inheritdoc />
        public bool Contains(string name) => _values.ContainsKey(name);
    }
}
