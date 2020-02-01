using NUnit.Framework;
using System;

namespace StringMath.Tests
{
    [TestFixture]
    internal class CalculatorTests
    {
        private Calculator _calculator;

        [Test]
        [TestCase("1 +\t 2", 3)]
        [TestCase("-1.5 + 3", 1.5)]
        [TestCase("4!", 24)]
        [TestCase("(3! + 1) * 2", 14)]
        [TestCase("2 ^ 3", 8)]
        [TestCase("1 + 16 log 2", 5)]
        [TestCase("1 + sqrt 4", 3)]
        [TestCase("((1 + 1) + ((1 + 1) + (((1) + 1)) + 1))", 7)]
        public void TestEvaluationResult(string input, decimal expected)
        {
            Assert.AreEqual(expected, _calculator.Evaluate(input));
        }

        [Test]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("{b}+3*{a}", 3, 11)]
        [TestCase("({a})", 3, 3)]
        [TestCase("{PI}", Math.PI, Math.PI)]
        [TestCase("{E}", Math.E, Math.E)]
        public void ReplacementEvaluationResult(string input, decimal replacement, decimal expected)
        {
            _calculator.Replace("a", replacement);
            _calculator.Replace("b", 2);

            Assert.AreEqual(expected, _calculator.Evaluate(input));
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _calculator = new Calculator();

            _calculator.AddOperator("abs", a => a > 0 ? a : -a);
            _calculator.AddOperator("x", (a, b) => a * b);
            _calculator.AddOperator("<<", (a, b) => (decimal)Math.ScaleB((double)a, (int)b));
            _calculator.AddOperator("<>", (a, b) => decimal.Parse($"{a}{b}"), Precedence.Prefix);
            _calculator.AddOperator("e", (a, b) => decimal.Parse($"{a}e{b}"), Precedence.Power);
        }

        [Test]
        [TestCase("abs -5", 5)]
        [TestCase("abs(-1)", 1)]
        [TestCase("3 max 2", 3)]
        [TestCase("2 x\r\n 5", 10)]
        [TestCase("3 << 2", 12)]
        [TestCase("-3 <> 2", -32)]
        public void CustomOperators(string input, decimal expected)
        {
            Assert.AreEqual(expected, _calculator.Evaluate(input));
        }
    }
}
