﻿using System;

namespace StringMath
{
    internal sealed class ReplacementExpression : Expression
    {
        public ReplacementExpression(string name)
            => Name = name;

        public string Name { get; }
        public override Type Type => typeof(ReplacementExpression);
    }
}
