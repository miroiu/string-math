using NUnit.Framework;
using System;

namespace StringMath.Tests
{
    [TestFixture]
    internal class CalculatorTests
    {
        private ICalculator<double> _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new Calculator<double>();

            _calculator.AddOperator("abs", a => a > 0 ? a : -a);
            _calculator.AddOperator("x", (a, b) => a * b);
            _calculator.AddOperator("<<", (a, b) => a * Math.Pow(2, (int)b));
            _calculator.AddOperator("<>", (a, b) => double.Parse($"{a}{b}"), Precedence.Prefix);
            _calculator.AddOperator("e", (a, b) => double.Parse($"{a}e{b}"), Precedence.Power);
            _calculator.AddOperator("sind", a => Math.Sin(a * (Math.PI / 180)));
        }

        [Test]
        [TestCase("1 +\t 2", 3)]
        [TestCase("-1.5 + 3", 1.5)]
        [TestCase("4!", 24)]
        [TestCase("(3! + 1) * 2", 14)]
        [TestCase("2 ^ 3", 8)]
        [TestCase("1 + 16 log 2", 5)]
        [TestCase("1 + sqrt 4", 3)]
        [TestCase("sind(90) + sind 30", 1.5)]
        [TestCase("((1 + 1) + ((1 + 1) + (((1) + 1)) + 1))", 7)]
        public void Evaluate(string input, double expected)
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
        public void Evaluate(string input, double variable, double expected)
        {
            _calculator["a"] = variable;
            _calculator["b"] = 2;

            Assert.AreEqual(expected, _calculator.Evaluate(input));
        }

        [Test]
        [TestCase("abs -5", 5)]
        [TestCase("abs(-1)", 1)]
        [TestCase("3 max 2", 3)]
        [TestCase("2 x\r\n 5", 10)]
        [TestCase("3 << 2", 12)]
        [TestCase("-3 <> 2", -32)]
        public void Evaluate_CustomOperators(string input, double expected)
        {
            Assert.AreEqual(expected, _calculator.Evaluate(input));
        }

        [Test]
        [TestCase("abs -5 + {a}", 1, 6)]
        [TestCase("{a} + 2 * abs(-1) / {a}", 1, 3)]
        [TestCase("3 max {a}", 2, 3)]
        [TestCase("2 x\r\n {a}", 5, 10)]
        [TestCase("{a} << {a}", 3, 24)]
        [TestCase("-3 <> {a}", 2, -32)]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("({a})", 3, 3)]
        [TestCase("2 * ({a} + 3 + 5)", 1, 18)]
        public void Evaluate_CachedOperation_With_Variables(string input, double repl, double expected)
        {
            _calculator.SetValue("a", repl);

            OperationInfo<double> op = _calculator.CreateOperation(input);

            Assert.That(op.Variables, Is.EquivalentTo(new[] { "a" }));
            Assert.AreEqual(input, op.Expression);

            Assert.AreEqual(expected, _calculator.Evaluate(input));
            Assert.AreEqual(expected, _calculator.Evaluate(op));

            _calculator.SetValue("a", 9);
            Assert.AreNotEqual(expected, _calculator.Evaluate(op));
        }

        [Test]
        [TestCase("abs -5", 5)]
        [TestCase("abs(-1)", 1)]
        [TestCase("3 max 2", 3)]
        [TestCase("2 x\r\n 5", 10)]
        [TestCase("3 << 2", 12)]
        [TestCase("-3 <> 2", -32)]
        public void Evaluate_CachedOperation_Without_Variables(string input, double expected)
        {
            OperationInfo<double> op = _calculator.CreateOperation(input);
            Assert.AreEqual(input, op.Expression);

            Assert.AreEqual(expected, _calculator.Evaluate(input));
            Assert.AreEqual(expected, _calculator.Evaluate(op));
        }

        [Test]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("{b}+3*{a}", 3, 11)]
        [TestCase("({a})", 3, 3)]
        public void Evaluate_Static_API(string input, double variable, double expected)
        {
            SMath<double>.SetValues(new VariablesCollection<double>
            {
                ["b"] = 2,
                ["a"] = variable
            });
            Assert.AreEqual(expected, SMath<double>.Evaluate(input));
        }

        [Test]
        [TestCase("{a}+2")]
        public void Evaluate_Unassigned_Variable_Exception(string input)
        {
            LangException exception = Assert.Throws<LangException>(() => _calculator.Evaluate(input));
            Assert.AreEqual(LangException.ErrorCode.UNASSIGNED_VARIABLE, exception.ErrorType);
        }
    }
}
