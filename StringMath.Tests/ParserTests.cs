using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class ParserTests
    {
        private MathContext _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new MathContext();
        }

        [Test]
        [TestCase("1 * 2", "1 * 2")]
        [TestCase("1 ^ (6 % 2)", "1 ^ (6 % 2)")]
        [TestCase("1 - (3 + 8)", "1 - (3 + 8)")]
        [TestCase("5! + ({a} / 3)", "5! + ({a} / 3)")]
        [TestCase("-9.53", "-9.53")]
        [TestCase("1.15215345346", "1.15215345346")]
        [TestCase("0", "0")]
        public void TestCorrectParsing(string input, string expected)
        {
            SourceText sourceText = new SourceText(input);
            Lexer lexer = new Lexer(sourceText, _context);
            Parser parser = new Parser(lexer, _context);

            Expression result = parser.Parse();
            string actual = result.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}