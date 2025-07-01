using NUnit.Framework;
using StringMath.Expressions;

namespace StringMath.Tests
{
    [TestFixture]
    internal class ParserTests
    {
        private IMathContext _context;

        [OneTimeSetUp]
        public void Setup()
        {
            _context = MathContext.Default;
        }

        [Test]
        [TestCase("1 * 2", "1 * 2")]
        [TestCase("1 ^ (6 % 2)", "1 ^ (6 % 2)")]
        [TestCase("1 - ((3 + 8) max 1)", "1 - (3 + 8) max 1")]
        [TestCase("5! + ({a} / 3)", "5! + {a} / 3")]
        [TestCase("-9.53", "-9.53")]
        [TestCase("1.15215345346", "1.15215345346")]
        [TestCase("0", "0")]
        [TestCase("!2", "2!")]
        [TestCase("--1", "-(-1)")]
        [TestCase("1+sin(3)", "1 + sin(3)")]
        [TestCase("1+sin 3", "1 + sin(3)")]
        public void ParseMathExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("1 2 * 2")]
        [TestCase("1 * 2 {a}")]
        [TestCase("1 * {}")]
        [TestCase("{}")]
        [TestCase("()")]
        [TestCase(")")]
        [TestCase("{a}{b}")]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\0")]
        [TestCase("\01+2")]
        [TestCase("\n")]
        [TestCase("\t")]
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("(1+2")]
        [TestCase("1+(2")]
        [TestCase("1+2)")]
        [TestCase("1+")]
        [TestCase("1.")]
        [TestCase("1..1")]
        [TestCase("-*1")]
        [TestCase("-+1")]
        [TestCase("+-1")]
        [TestCase("{")]
        [TestCase("}")]
        [TestCase("asd")]
        [TestCase("1 + asd")]
        [TestCase("{-a}")]
        [TestCase("*{a}")]
        [TestCase("1 + 2 1")]
        public void ParseBadExpression_Exception(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.AreEqual(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Test]
        [TestCase("{a} pow 3", "{a} pow 3")]
        [TestCase("rand (3 + 5)", "rand(3 + 5)")]
        [TestCase("rand (3 pow 5)", "rand(3 pow 5)")]
        public void ParseExpression_CustomOperators(string input, string expected)
        {
            MathContext context = new MathContext(_context);
            context.RegisterBinary("pow", (a, b) => a);
            context.RegisterUnary("rand", (a) => a);

            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, context);

            IExpression result = parser.Parse();
            string actual = result.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("{a} pow 3")]
        [TestCase("rand 3")]
        public void ParseExpression_CustomOperators_Exception(string expected)
        {
            MathContext context = new MathContext();

            Tokenizer tokenizer = new Tokenizer(expected, _context);
            Parser parser = new Parser(tokenizer, context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.AreEqual(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Test]
        [TestCase("{a}", "a")]
        [TestCase("{var}", "var")]
        [TestCase("{__var}", "__var")]
        [TestCase("{_}", "_")]
        [TestCase("{_12}", "_12")]
        [TestCase("{a_}", "a_")]
        [TestCase("{a_a}", "a_a")]
        [TestCase("{a123_}", "a123_")]
        [TestCase("{a13}", "a13")]
        public void ParseVariableExpression(string expected, string name)
        {
            Tokenizer tokenizer = new Tokenizer(expected, _context);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString();

            Assert.IsInstanceOf<VariableExpression>(result);
            Assert.AreEqual(name, ((VariableExpression)result).Name);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("{}")]
        [TestCase("{1a}")]
        [TestCase("{123}")]
        [TestCase("{ }")]
        [TestCase("{ a }")]
        [TestCase("{-a}")]
        public void ParseVariableExpression_Exception(string expected)
        {
            Tokenizer tokenizer = new Tokenizer(expected, _context);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.AreEqual(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Test]
        [TestCase("10 + 3", "10 + 3")]
        [TestCase("cos(90.0) - 5", "cos(90) - 5")]
        [TestCase("round(1) * (2 + 3)", "round(1) * (2 + 3)")]
        [TestCase("!1.5 / 3", "1.5! / 3")]
        [TestCase("1.5! ^ sqrt 3", "1.5! ^ sqrt(3)")]
        [TestCase("{a} - abs (5 % 3)", "{a} - abs(5 % 3)")]
        [TestCase("{a} - 3 + {b}", "{a} - 3 + {b}")]
        [TestCase("{a} / 3 * {b}", "{a} / 3 * {b}")]
        [TestCase("1 + 2 + 3", "1 + 2 + 3")]
        [TestCase("1 / 2 / 3", "1 / 2 / 3")]
        public void ParseBinaryExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.IsInstanceOf<BinaryExpression>(result);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("sin(90.0 + 2)", "sin(90 + 2)")]
        [TestCase("cos(90.0)", "cos(90)")]
        [TestCase("round(1)", "round(1)")]
        [TestCase("!1.5", "1.5!")]
        [TestCase("1.5!", "1.5!")]
        [TestCase("abs.5", "abs(0.5)")]
        [TestCase("-999", "-999")]
        [TestCase("sqrt(-999 / 2 * 3 max 5)", "sqrt(-999 / 2 * 3 max 5)")]
        [TestCase("-(sqrt5)", "-(sqrt(5))")]
        [TestCase("- sqrt5", "-(sqrt(5))")]
        [TestCase("sqrt{a}", "sqrt({a})")]
        public void ParseUnaryExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.IsInstanceOf<UnaryExpression>(result);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("fail 5")]
        [TestCase("-+5")]
        [TestCase("+5")]
        public void ParseUnaryExpression_Exception(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.AreEqual(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("1.5", "1.5")]
        [TestCase(".5", "0.5")]
        [TestCase("9999999", "9999999")]
        public void ParseConstantExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.IsInstanceOf<ConstantExpression>(result);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("1a")]
        [TestCase("1.a")]
        [TestCase(".5a")]
        [TestCase(".a")]
        [TestCase("9.01+")]
        public void ParseConstantExpression_Exception(string expected)
        {
            Tokenizer tokenizer = new Tokenizer(expected, _context);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.AreEqual(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Test]
        [TestCase("(1)", "1")]
        [TestCase("((1))", "1")]
        [TestCase("(1 + 2)", "1 + 2")]
        [TestCase("((1 + 2) % 3)", "(1 + 2) % 3")]
        [TestCase("(5! * (1 + 2))", "5! * (1 + 2)")]
        [TestCase("(5 + (1 + 2))", "5 + 1 + 2")]
        [TestCase("((5 - {a}) + (1 + 2))", "5 - {a} + 1 + 2")]
        [TestCase("((5 - 2) + (1 + 2! * 3))", "5 - 2 + 1 + 2! * 3")]
        [TestCase("((5 - 2) + ((-1 + 2) * 3))", "5 - 2 + (-1 + 2) * 3")]
        public void ParseGroupingExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input, _context);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("()")]
        [TestCase("(")]
        [TestCase("(1")]
        [TestCase("(1")]
        [TestCase("((1)")]
        [TestCase("1 + 2(")]
        [TestCase("(1 + 2 * 3")]
        [TestCase("5 - 2( + (1 + 2)")]
        [TestCase("({a} + (1 + 2)")]
        public void ParseGroupingExpression_Fail(string expected)
        {
            Tokenizer tokenizer = new Tokenizer(expected, _context);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.AreEqual(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }
    }
}