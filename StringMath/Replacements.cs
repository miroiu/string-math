﻿using System.Collections.Generic;

namespace StringMath
{
    public class Replacements : Dictionary<string, decimal>
    {
        /// <summary>
        /// Overwrites the value of a variable.
        /// </summary>
        /// <param name="name">The variable's name.</param>
        /// <param name="value">The variable's value.</param>
        public new void Add(string name, decimal value)
            => base[name] = value;
    }
}
