using NUnit.Framework;
using System;

namespace StringMath.Tests
{
    [TestFixture]
    internal class SMathTests
    {
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
            Assert.AreEqual(expected, SMath.Evaluate(input));
        }

        [Test]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("{b}+3*{a}", 3, 11)]
        [TestCase("({a})", 3, 3)]
        public void ReplacementEvaluationResult(string input, decimal replacement, decimal expected)
        {
            Assert.AreEqual(expected, SMath.Evaluate(input, new Replacements
            {
                ["a"] = replacement,
                ["b"] = 2,
            }));
        }

        [OneTimeSetUp]
        public void Setup()
        {
            SMath.AddOperator("abs", a => a > 0 ? a : -a);
            SMath.AddOperator("max", (a, b) => a > b ? a : b);
            SMath.AddOperator("x", (a, b) => a * b);
            SMath.AddOperator("<<", (a, b) => (decimal)Math.ScaleB((double)a, (int)b));
            SMath.AddOperator("<>", (a, b) => decimal.Parse($"{a}{b}"), Precedence.Prefix);
            SMath.AddOperator("e", (a, b) => decimal.Parse($"{a}e{b}"), Precedence.Power);
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
            Assert.AreEqual(expected, SMath.Evaluate(input));
        }
    }
}
