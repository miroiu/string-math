using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class EvaluatorTest
    {
        [Test]
        [TestCase("1 + 2", 3)]
        [TestCase("-1.5 + 3", 1.5)]
        public void TestEvaluationResult(string input, decimal expected)
        {
            Assert.AreEqual(expected, (decimal)Math.Evaluate(input));
        }
    }
}
