using System.Collections.Generic;

namespace StringMath
{
    public interface IVariablesCollection
    {
        void Set(string name, double value);
        double Get(string name);
        bool TryGetValue(string name, out double value);
    }

    public class VariablesCollection : Dictionary<string, double>, IVariablesCollection
    {
        /// <summary>
        /// Overwrites the value of a variable.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's value.</param>
        public new void Add(string name, double value)
        {
            base[name] = value;
        }

        public double Get(string name)
        {
            return base[name];
        }

        public void Set(string name, double value)
        {
            Add(name, value);
        }
    }
}
