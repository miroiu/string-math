namespace StringMath.Expressions.Tests
{
    public class ExpressionsTests
    {
        [Theory]
        [InlineData("5 * (3 + 2)", "5 * 3 + 5 * 2")]
        [InlineData("5 * (3 - 2)", "5 * 3 - 5 * 2")]
        [InlineData("{a} * (3 + {b})", "{a} * 3 + {a} * {b}")]
        [InlineData("{a} * (3 - {b})", "{a} * 3 - {a} * {b}")]
        public void Expand(string input, string expected)
        {
            var actual = input.Expand();

            Assert.Equal(expected, actual.Text);
        }

        [Theory]
        [InlineData("5^2", "25")]
        [InlineData("(5 - 2) * {a}", "3 * {a}")]
        [InlineData("(-5 + 2) * {a}", "-3 * {a}")]
        [InlineData("-(-5 + 2) * {a}", "3 * {a}")]
        [InlineData("-(-5 + 1)", "4")]
        [InlineData("-(-5 + -1)", "6")]
        [InlineData("-{a} + 2", "-{a} + 2")]
        public void Simplify(string input, string expected)
        {
            var actual = input.Simplify();

            Assert.Equal(expected, actual.Text);
        }
    }
}
