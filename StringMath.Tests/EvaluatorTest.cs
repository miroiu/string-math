using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class EvaluatorTest
    {
        [Test]
        [TestCase("1 + 2", 3)]
        [TestCase("-1.5 + 3", 1.5)]
        [TestCase("4!", 24)]
        [TestCase("(3! + 1) * 2", 14)]
        [TestCase("2^3", 8)]
        public void TestEvaluationResult(string input, decimal expected)
        {
            Assert.AreEqual(expected, Calculator.Evaluate(input));
        }

        [Test]
        [TestCase("{a}+2", 1, 3)]
        [TestCase("2*{a}+2", 3, 8)]
        [TestCase("2*{a}+2*{a}", 3, 12)]
        [TestCase("{b}+3*{a}", 3, 11)]
        public void ReplacementEvaluationResult(string input, decimal replacement, decimal expected)
        {
            Assert.AreEqual(expected, Calculator.Evaluate(input, new Replacement("a", replacement), new Replacement("b", 2)));
        }

        [SetUp]
        public void Setup()
        {
            Calculator.AddUnaryOperator("abs", a => a > 0 ? a : -a);
            Calculator.AddBinaryOperator("max", (a, b) => a > b ? a : b);
        }

        [Test]
        [TestCase("abs-5", 5)]
        [TestCase("abs(-1)", 1)]
        [TestCase("3max2", 3)]
        public void CustomOperators(string input, decimal expected)
        {
            Assert.AreEqual(expected, Calculator.Evaluate(input));
        }
    }
}
