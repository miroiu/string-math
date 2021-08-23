using System.Collections.Generic;

namespace StringMath
{
    public class VariablesCollection : Dictionary<string, double>
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
    }
}
