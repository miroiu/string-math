using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class ParserTests
    {
        private IMathContext _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new MathContext();
        }

        [Test]
        [TestCase("1 * 2", "1 * 2")]
        [TestCase("1 ^ (6 % 2)", "1 ^ (6 % 2)")]
        [TestCase("1 - ((3 + 8) max 1)", "1 - ((3 + 8) max 1)")]
        [TestCase("5! + ({a} / 3)", "5! + ({a} / 3)")]
        [TestCase("-9.53", "-9.53")]
        [TestCase("1.15215345346", "1.15215345346")]
        [TestCase("0", "0")]
        [TestCase("!2", "2!")]
        public void TestCorrectParsing(string input, string expected)
        {
            ISourceText sourceText = new SourceText(input);
            ILexer lexer = new Lexer(sourceText, _context);
            IParser parser = new Parser(lexer, _context);

            Expression result = parser.Parse();
            string actual = result.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("1 2 * 2")]
        [TestCase("1 * 2 {a}")]
        [TestCase("1 * {}")]
        [TestCase("{}")]
        [TestCase("()")]
        [TestCase("{a}{b}")]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\0")]
        [TestCase("\n")]
        [TestCase("\t")]
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("(1+2")]
        [TestCase("1+(2")]
        [TestCase("1+2)")]
        [TestCase("1+")]
        [TestCase("1.")]
        [TestCase(".1")]
        [TestCase("1..1")]
        [TestCase("--1")]
        [TestCase("-+1")]
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("asd")]
        [TestCase("1 + asd")]
        [TestCase("{-a}")]
        [TestCase("*{a}")]
        public void TestCorrectParsingBadExpression(string input)
        {
            ISourceText sourceText = new SourceText(input);
            ILexer lexer = new Lexer(sourceText, _context);
            IParser parser = new Parser(lexer, _context);

            Assert.Throws<LangException>(() => parser.Parse());
        }
    }
}