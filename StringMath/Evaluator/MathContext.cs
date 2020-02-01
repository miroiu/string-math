using System;
using System.Collections.Generic;

namespace StringMath
{
    internal sealed class MathContext
    {
        private readonly Dictionary<string, Func<decimal, decimal, decimal>> _binaryEvaluators = new Dictionary<string, Func<decimal, decimal, decimal>>();
        private readonly Dictionary<string, Func<decimal, decimal>> _unaryEvaluators = new Dictionary<string, Func<decimal, decimal>>();
        private readonly Dictionary<string, Precedence> _binaryPrecedence = new Dictionary<string, Precedence>();
        private readonly HashSet<string> _operators = new HashSet<string>();

        public MathContext()
        {
            AddBinaryOperator("+", (a, b) => a + b, Precedence.Addition);
            AddBinaryOperator("-", (a, b) => a - b, Precedence.Addition);
            AddBinaryOperator("*", (a, b) => a * b, Precedence.Multiplication);
            AddBinaryOperator("/", (a, b) => a / b, Precedence.Multiplication);
            AddBinaryOperator("%", (a, b) => a % b, Precedence.Multiplication);
            AddBinaryOperator("^", (a, b) => (decimal)Math.Pow((double)a, (double)b), Precedence.Power);
            AddBinaryOperator("log", (a, b) => (decimal)Math.Log((double)a, (double)b), Precedence.Logarithmic);
            AddBinaryOperator("max", (a, b) => Math.Max(a, b), Precedence.UserDefined);
            AddBinaryOperator("min", (a, b) => Math.Min(a, b), Precedence.UserDefined);

            AddUnaryOperator("-", a => -a);
            AddUnaryOperator("!", a => ComputeFactorial(a));
            AddUnaryOperator("sqrt", a => (decimal)Math.Sqrt((double)a));
            AddUnaryOperator("sin", a => (decimal)Math.Sin((double)a));
            AddUnaryOperator("cos", a => (decimal)Math.Cos((double)a));
            AddUnaryOperator("tan", a => (decimal)Math.Tan((double)a));
            AddUnaryOperator("ceil", a => Math.Ceiling(a));
            AddUnaryOperator("floor", a => Math.Floor(a));
            AddUnaryOperator("round", a => Math.Round(a));
            AddUnaryOperator("exp", a => (decimal)Math.Exp((double)a));
            AddUnaryOperator("abs", a => Math.Abs(a));
        }

        public bool IsUnaryOperator(string operatorName)
            => _unaryEvaluators.ContainsKey(operatorName);

        public bool IsBinaryOperator(string operatorName)
            => _binaryEvaluators.ContainsKey(operatorName);

        public bool IsOperator(string operatorName)
            => _operators.Contains(operatorName);

        public Precedence GetBinaryOperatorPrecedence(string operatorName)
            => _binaryPrecedence[operatorName];

        public void AddBinaryOperator(string operatorName, Func<decimal, decimal, decimal> operation, Precedence precedence = default)
        {
            _binaryEvaluators[operatorName] = operation;
            _binaryPrecedence[operatorName] = precedence ?? Precedence.Logarithmic;
            _operators.Add(operatorName);
        }

        public void AddUnaryOperator(string operatorName, Func<decimal, decimal> operation)
        {
            _unaryEvaluators[operatorName] = operation;
            _operators.Add(operatorName);
        }

        public decimal EvaluateBinary(string op, decimal a, decimal b)
            => _binaryEvaluators[op](a, b);

        public decimal EvaluateUnary(string op, decimal a)
            => _unaryEvaluators[op](a);

        #region Factorial

        private static readonly Dictionary<decimal, decimal> _factorials = new Dictionary<decimal, decimal>
        {
            [0] = 1m,
            [1] = 1m,
            [2] = 2m,
            [3] = 6m,
            [4] = 24m,
            [5] = 120m,
            [6] = 720m,
            [8] = 40320m,
            [9] = 362880m,
            [10] = 3628800m,
            [11] = 39916800m,
            [12] = 479001600m,
            [13] = 6227020800m,
            [14] = 87178291200m,
            [15] = 1307674368000m,
            [16] = 20922789888000m,
            [17] = 355687428096000m,
            [18] = 6402373705728000m,
            [19] = 121645100408832000m,
            [20] = 2432902008176640000m,
            [21] = 5.1090942e+19m,
            [22] = 1.1240007e+21m,
            [23] = 2.5852017e+22m,
            [24] = 6.204484e+23m,
            [25] = 1.551121e+25m,
            [26] = 4.0329146e+26m,
            [27] = 1.0888869e+28m
        };

        private static decimal ComputeFactorial(decimal value)
        {
            if (value > 27)
            {
                throw new InvalidOperationException("Result is too big.");
            }
            else if (value < 0)
            {
                throw new ArgumentException("Value is negative.");
            }

            return _factorials[value];
        }

        #endregion
    }
}
