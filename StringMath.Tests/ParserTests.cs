using Xunit;
using StringMath.Expressions;

namespace StringMath.Tests
{
    public class ParserTests
    {
        private readonly IMathContext _context;

        public ParserTests()
        {
            _context = MathContext.Default;
        }

        [Theory]
        [InlineData("1 * 2", "1 * 2")]
        [InlineData("1 ^ (6 % 2)", "1 ^ (6 % 2)")]
        [InlineData("1 - ((3 + 8) max 1)", "1 - (3 + 8) max 1")]
        [InlineData("5! + ({a} / 3)", "5! + {a} / 3")]
        [InlineData("-9.53", "-9.53")]
        [InlineData("1.15215345346", "1.15215345346")]
        [InlineData("0", "0")]
        [InlineData("!2", "2!")]
        public void ParseMathExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("1 2 * 2")]
        [InlineData("1 * 2 {a}")]
        [InlineData("()")]
        [InlineData(")")]
        [InlineData("{a}{b}")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\0")]
        [InlineData("\01+2")]
        [InlineData("\n")]
        [InlineData("\t")]
        [InlineData("(1+2")]
        [InlineData("1+(2")]
        [InlineData("1+2)")]
        [InlineData("1+")]
        [InlineData("--1")]
        [InlineData("-+1")]
        [InlineData("asd")]
        [InlineData("1 + asd")]
        [InlineData("*{a}")]
        [InlineData("1 + 2 1")]
        [InlineData("9.01+")]
        [InlineData(".5a")]
        public void ParseBadExpression_Exception(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.Equal(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Theory]
        [InlineData("{a} pow 3", "{a} pow 3")]
        [InlineData("rand (3 + 5)", "rand(3 + 5)")]
        [InlineData("rand (3 pow 5)", "rand(3 pow 5)")]
        public void ParseExpression_CustomOperators(string input, string expected)
        {
            MathContext context = new MathContext(_context);
            context.RegisterBinary("pow", (a, b) => a);
            context.RegisterUnary("rand", (a) => a);

            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, context);

            IExpression result = parser.Parse();
            string actual = result.ToString();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("{a} pow 3")]
        [InlineData("rand 3")]
        public void ParseExpression_CustomOperators_Exception(string expected)
        {
            MathContext context = new MathContext();

            Tokenizer tokenizer = new Tokenizer(expected);
            Parser parser = new Parser(tokenizer, context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.Equal(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Theory]
        [InlineData("{a}", "a")]
        [InlineData("{var}", "var")]
        [InlineData("{__var}", "__var")]
        [InlineData("{_}", "_")]
        [InlineData("{_12}", "_12")]
        [InlineData("{a_}", "a_")]
        [InlineData("{a_a}", "a_a")]
        [InlineData("{a123_}", "a123_")]
        [InlineData("{a13}", "a13")]
        public void ParseVariableExpression(string expected, string name)
        {
            Tokenizer tokenizer = new Tokenizer(expected);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString();

            Assert.IsType<VariableExpression>(result);
            Assert.Equal(name, ((VariableExpression)result).Name);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("10 + 3", "10 + 3")]
        [InlineData("cos(90.0) - 5", "cos(90) - 5")]
        [InlineData("round(1) * (2 + 3)", "round(1) * (2 + 3)")]
        [InlineData("!1.5 / 3", "1.5! / 3")]
        [InlineData("1.5! ^ sqrt 3", "1.5! ^ sqrt(3)")]
        [InlineData("{a} - abs (5 % 3)", "{a} - abs(5 % 3)")]
        [InlineData("{a} - 3 + {b}", "{a} - 3 + {b}")]
        [InlineData("{a} / 3 * {b}", "{a} / 3 * {b}")]
        [InlineData("1 + 2 + 3", "1 + 2 + 3")]
        [InlineData("1 / 2 / 3", "1 / 2 / 3")]
        public void ParseBinaryExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.IsType<BinaryExpression>(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("sin(90.0 + 2)", "sin(90 + 2)")]
        [InlineData("cos(90.0)", "cos(90)")]
        [InlineData("round(1)", "round(1)")]
        [InlineData("!1.5", "1.5!")]
        [InlineData("1.5!", "1.5!")]
        [InlineData("abs.5", "abs(0.5)")]
        [InlineData("-999", "-999")]
        [InlineData("sqrt(-999 / 2 * 3 max 5)", "sqrt(-999 / 2 * 3 max 5)")]
        [InlineData("-(sqrt5)", "-(sqrt(5))")]
        [InlineData("- sqrt5", "-(sqrt(5))")]
        [InlineData("sqrt{a}", "sqrt({a})")]
        public void ParseUnaryExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.IsType<UnaryExpression>(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("fail 5")]
        [InlineData("-+5")]
        [InlineData("+5")]
        public void ParseUnaryExpression_Exception(string input)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.Equal(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }

        [Theory]
        [InlineData("1", "1")]
        [InlineData("1.5", "1.5")]
        [InlineData(".5", "0.5")]
        [InlineData("9999999", "9999999")]
        public void ParseConstantExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("(1)", "1")]
        [InlineData("((1))", "1")]
        [InlineData("(1 + 2)", "1 + 2")]
        [InlineData("((1 + 2) % 3)", "(1 + 2) % 3")]
        [InlineData("(5! * (1 + 2))", "5! * (1 + 2)")]
        [InlineData("(5 + (1 + 2))", "5 + 1 + 2")]
        [InlineData("((5 - {a}) + (1 + 2))", "5 - {a} + 1 + 2")]
        [InlineData("((5 - 2) + (1 + 2! * 3))", "5 - 2 + 1 + 2! * 3")]
        [InlineData("((5 - 2) + ((-1 + 2) * 3))", "5 - 2 + (-1 + 2) * 3")]
        public void ParseGroupingExpression(string input, string expected)
        {
            Tokenizer tokenizer = new Tokenizer(input);
            Parser parser = new Parser(tokenizer, _context);

            IExpression result = parser.Parse();
            string actual = result.ToString(_context);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("()")]
        [InlineData("(")]
        [InlineData("(1")]
        [InlineData("((1)")]
        [InlineData("1 + 2(")]
        [InlineData("(1 + 2 * 3")]
        [InlineData("5 - 2( + (1 + 2)")]
        [InlineData("({a} + (1 + 2)")]
        public void ParseGroupingExpression_Fail(string expected)
        {
            Tokenizer tokenizer = new Tokenizer(expected);
            Parser parser = new Parser(tokenizer, _context);

            MathException exception = Assert.Throws<MathException>(() => parser.Parse());
            Assert.Equal(MathException.ErrorCode.UNEXPECTED_TOKEN, exception.Code);
        }
    }
}