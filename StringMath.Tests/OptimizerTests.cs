using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class OptimizerTests
    {
        private MathContext _context;
        private Reducer _reducer;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new MathContext();
            _reducer = new Reducer();
        }

        [Test]
        [TestCase("1 * 2", "2")]
        [TestCase("2 ^ (6 % 2)", "1")]
        [TestCase("1 - (3 + 8)", "-10")]
        [TestCase("-9.53", "-9.53")]
        [TestCase("1.15215345346", "1.15215345346")]
        [TestCase("0", "0")]
        [TestCase("5! + ({a} / 3)", "120 + {a} / 3")]
        [TestCase("2 + {a} + 3", "2 + {a} + 3")]
        [TestCase("3! + 3 + {a}", "9 + {a}")]
        [TestCase("3! + 5! + {a}", "126 + {a}")]
        public void TestCorrectOptimizing(string input, string expected)
        {
            SourceText sourceText = new SourceText(input);
            Lexer lexer = new Lexer(sourceText, _context);
            Parser parser = new Parser(lexer, _context);
            Optimizer optimizer = new Optimizer(_reducer, _context);

            Expression parsedExpr = parser.Parse();
            Expression optimizedExpr = optimizer.Optimize(parsedExpr);
            string actual = optimizedExpr.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}