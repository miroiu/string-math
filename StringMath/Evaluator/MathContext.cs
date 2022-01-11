using System;
using System.Collections.Generic;

namespace StringMath
{
    /// <inheritdoc />
    internal sealed class MathContext<TNum> : IMathContext<TNum> where TNum : INumber<TNum>
    {
        private readonly Dictionary<string, Func<TNum, TNum, TNum>> _binaryEvaluators = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Func<TNum, TNum>> _unaryEvaluators = new(StringComparer.Ordinal);
        private readonly Dictionary<string, Precedence> _binaryPrecedence = new(StringComparer.Ordinal);
        private readonly HashSet<string> _operators = new(StringComparer.Ordinal);

        /// <summary>Initializez a new instance of a math context.</summary>
        public MathContext()
        {
            RegisterBinary("+", (a, b) => a + b, Precedence.Addition);
            RegisterBinary("-", (a, b) => a - b, Precedence.Addition);
            RegisterBinary("*", (a, b) => a * b, Precedence.Multiplication);
            RegisterBinary("/", (a, b) => a / b, Precedence.Multiplication);
            RegisterBinary("%", (a, b) => a % b, Precedence.Multiplication);

            RegisterBinary("max", (a, b) => TNum.Max(a, b), Precedence.UserDefined);
            RegisterBinary("min", (a, b) => TNum.Min(a, b), Precedence.UserDefined);
            RegisterBinary("^", (a, b) => TNum.Create(Math.Pow(Convert.ToDouble(a), Convert.ToDouble(b))), Precedence.Power);
            RegisterBinary("log", (a, b) => TNum.Create(Math.Log(Convert.ToDouble(a), Convert.ToDouble(b))), Precedence.Logarithmic);

            RegisterUnary("-", a => -a);
            RegisterUnary("!", a => a.ToFactorial());
            RegisterUnary("sign", a => TNum.Sign(a));
            RegisterUnary("sqrt", a => TNum.Create(Math.Sqrt(Convert.ToDouble(a))));
            RegisterUnary("sin", a => TNum.Create(Math.Sin(Convert.ToDouble(a))));
            RegisterUnary("cos", a => TNum.Create(Math.Cos(Convert.ToDouble(a))));
            RegisterUnary("tan", a => TNum.Create(Math.Tan(Convert.ToDouble(a))));
            RegisterUnary("ceil", a => TNum.Create(Math.Ceiling(Convert.ToDouble(a))));
            RegisterUnary("floor", a => TNum.Create(Math.Floor(Convert.ToDouble(a))));
            RegisterUnary("round", a => TNum.Create(Math.Round(Convert.ToDouble(a))));
            RegisterUnary("exp", a => TNum.Create(Math.Exp(Convert.ToDouble(a))));
            RegisterUnary("abs", a => TNum.Abs(a));
        }

        /// <inheritdoc />
        public bool IsUnary(string operatorName)
        {
            return _unaryEvaluators.ContainsKey(operatorName);
        }

        /// <inheritdoc />
        public bool IsBinary(string operatorName)
        {
            return _binaryEvaluators.ContainsKey(operatorName);
        }

        /// <inheritdoc />
        public Precedence GetBinaryPrecedence(string operatorName)
        {
            return _binaryPrecedence[operatorName];
        }

        /// <inheritdoc />
        public void RegisterBinary(string operatorName, Func<TNum, TNum, TNum> operation, Precedence? precedence = default)
        {
            _binaryEvaluators[operatorName] = operation;
            _binaryPrecedence[operatorName] = precedence ?? Precedence.UserDefined;
            _operators.Add(operatorName);
        }

        /// <inheritdoc />
        public void RegisterUnary(string operatorName, Func<TNum, TNum> operation)
        {
            _unaryEvaluators[operatorName] = operation;
            _operators.Add(operatorName);
        }

        /// <inheritdoc />
        public TNum EvaluateBinary(string op, TNum a, TNum b)
        {
            return _binaryEvaluators[op](a, b);
        }

        /// <inheritdoc />
        public TNum EvaluateUnary(string op, TNum a)
        {
            return _unaryEvaluators[op](a);
        }
    }
}
