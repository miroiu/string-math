using NUnit.Framework;

namespace StringMath.Tests
{
    [TestFixture]
    internal class OptimizerTests
    {
        private IMathContext<double> _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new MathContext<double>();
        }

        [Test]
        [TestCase("1 * 2", "2")]
        [TestCase("2 ^ (6 % 2)", "1")]
        [TestCase("1 - (3 + 8)", "-10")]
        [TestCase("-9.53", "-9.53")]
        [TestCase("1.15215345346", "1.15215345346")]
        [TestCase("0", "0")]
        [TestCase("5! + ({a} / 3)", "120 + {a} / 3")]
        [TestCase("3! + 3 + {a}", "9 + {a}")]
        [TestCase("3! + 5! + {a}", "126 + {a}")]
        public void Optimize(string input, string expected)
        {
            ITokenizer tokenizer = new Tokenizer(input);
            IParser parser = new Parser(tokenizer, _context);
            IExpressionVisitor<IExpression> optimizer = new ExpressionOptimizer<double>(_context);

            IExpression parsedExpr = parser.Parse();
            IExpression optimizedExpr = optimizer.Visit(parsedExpr);
            string actual = optimizedExpr.ToString();

            Assert.AreEqual(expected, actual);
        }

        // These tests could also pass if optimizer was smarter
        [Test]
        [TestCase("2 + {a} + 3", "5 + {a}")]
        [TestCase("2 * ({a} + 3 + 5)", "2 * ({a} + 8)")]
        [TestCase("{a} - 1 - 1", "{a} - 2")]
        public void Optimize_Fails(string input, string expected)
        {
            ITokenizer tokenizer = new Tokenizer(input);
            IParser parser = new Parser(tokenizer, _context);
            IExpressionVisitor<IExpression> optimizer = new ExpressionOptimizer<double>(_context);

            IExpression parsedExpr = parser.Parse();
            IExpression optimizedExpr = optimizer.Visit(parsedExpr);
            string actual = optimizedExpr.ToString();

            Assert.AreNotEqual(expected, actual);
        }
    }
}