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
            Assert.AreEqual(expected, (decimal)Calculator.Evaluate(input));
        }
    }
}
