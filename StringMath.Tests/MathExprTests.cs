using Xunit;
using System;

namespace StringMath.Tests
{
    public class MathExprTests
    {
        public MathExprTests()
        {
            MathExpr.AddOperator("abs", a => a > 0 ? a : -a);
            MathExpr.AddOperator("x", (a, b) => a * b);
            MathExpr.AddOperator("<<", (a, b) => a * Math.Pow(2, (int)b));
            MathExpr.AddOperator("<>", (a, b) => double.Parse($"{a}{b}"), Precedence.Prefix);
            MathExpr.AddOperator("e", (a, b) => double.Parse($"{a}e{b}"), Precedence.Power);
            MathExpr.AddOperator("sind", a => Math.Sin(a * (Math.PI / 180)));
        }

        [Fact]
        public void Substitute_Should_Not_Overwrite_Global_Variable()
        {
            MathExpr.AddVariable("PI", Math.PI);
            MathExpr expr = "3 + {PI}";

            MathException ex = Assert.Throws<MathException>(() => expr.Substitute("PI", 1));
            Assert.Equal(MathException.ErrorCode.READONLY_VARIABLE, ex.Code);
        }

        [Fact]
        public void Substitute_Should_Not_Set_Missing_Variable()
        {
            MathExpr expr = "3 + 2";

            MathException ex = Assert.Throws<MathException>(() => expr.Substitute("a", 1));
            Assert.Equal(MathException.ErrorCode.UNEXISTING_VARIABLE, ex.Code);
        }

        [Fact]
        public void Substitute_Should_Set_Local_Variable()
        {
            MathExpr expr = "3 + {a}";
            expr.Substitute("a", 1);

            Assert.Equal(4, expr.Result);
        }

        [Fact]
        public void Substitute_Should_Clear_Cache()
        {
            MathExpr expr = "{a} + 3";
            expr.Substitute("a", 3);

            Assert.Equal(6, expr.Result);

            expr.Substitute("a", 2);
            Assert.Equal(5, expr.Result);
        }

        [Fact]
        public void Indexer_Should_Set_Local_Variable()
        {
            MathExpr expr = "3 + {a} + {b}";
            expr["a"] = 1;
            expr["b"] = 2;

            Assert.Equal(6, expr.Result);
        }

        [Fact]
        public void Double_Conversion_Should_Evaluate_Expression()
        {
            double result = (MathExpr)"3 + 5";

            Assert.Equal(8, result);
        }

        [Theory]
        [InlineData("{a}", new[] { "a" })]
        [InlineData("2 * {a} - {PI}", new[] { "a" })]
        [InlineData("({a} - 5) * 4 + {E}", new[] { "a" })]
        public void LocalVariables_Should_Exclude_Global_Variables(string input, string[] expected)
        {
            MathExpr expr = input;

            Assert.Equal(expected, expr.LocalVariables);
        }

        [Theory]
        [InlineData("{a}", new[] { "a" })]
        [InlineData("2 * {a} - {PI}", new[] { "a", "PI" })]
        [InlineData("({a} - 5) * 4 + {E}", new[] { "a", "E" })]
        public void Variables_Should_Include_Global_Variables(string input, string[] expected)
        {
            MathExpr expr = input;

            Assert.Equal(expected, expr.Variables);
        }

        [Fact]
        public void SetOperator_Binary_Should_Overwrite_Specified_Operator()
        {
            MathExpr.AddOperator("*", (x, y) => x * y);
            MathExpr expr = "5 * 3";
            expr.SetOperator("*", (x, y) => x);

            Assert.Equal(5, expr.Result);
        }

        [Fact]
        public void SetOperator_Binary_Should_Not_Overwrite_Global_Operator()
        {
            MathExpr.AddOperator("*", (x, y) => x * y);
            MathExpr expr = "5 * 3";
            expr.SetOperator("*", (x, y) => x);

            MathExpr expr2 = "4 * 3";

            Assert.Equal(5, expr.Result);
            Assert.Equal(12, expr2.Result);
        }

        [Fact]
        public void SetOperator_Unary_Should_Overwrite_Specified_Operator()
        {
            MathExpr.AddOperator(">>", (x) => x * x);
            MathExpr expr = ">> 5";
            expr.SetOperator(">>", (x) => x);

            Assert.Equal(5, expr.Result);
        }

        [Fact]
        public void SetOperator_Unary_Should_Not_Overwrite_Global_Operator()
        {
            MathExpr.AddOperator("-", (x) => -x);
            MathExpr expr = "-5";
            expr.SetOperator("-", (x) => x);

            MathExpr expr2 = "-5";

            Assert.Equal(5, expr.Result);
            Assert.Equal(-5, expr2.Result);
        }

        [Theory]
        [InlineData("1 +\t 2", 3)]
        [InlineData("-1.5 + 3", 1.5)]
        [InlineData("4!", 24)]
        [InlineData("(4 + 1)!", 120)]
        [InlineData("(3! + 1) * 2", 14)]
        [InlineData("2 ^ 3", 8)]
        [InlineData("1 + 16 log 2", 5)]
        [InlineData("1 + sqrt 4", 3)]
        [InlineData("sind(90) + sind 30", 1.5)]
        [InlineData("((1 + 1) + ((1 + 1) + (((1) + 1)) + 1))", 7)]
        public void Evaluate(string input, double expected)
        {
            double result = input.Eval();
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("{b}+3*{a}", 3, 2, 11)]
        public void Evaluate_Using_Local_Variables(string input, double a, double b, double expected)
        {
            MathExpr expr = input;
            expr["a"] = a;
            expr["b"] = b;

            Assert.Equal(expected, expr.Result);
        }

        [Fact]
        public void Evaluate_Using_GlobalVariables()
        {
            MathExpr.AddVariable("PI", Math.PI);
            MathExpr expr = "{PI}";

            Assert.Equal(Math.PI, expr.Result);
        }

        [Theory]
        [InlineData("{a}+2", 1, 3)]
        [InlineData("2*{a}+2", 3, 8)]
        [InlineData("2*{a}+2*{a}", 3, 12)]
        [InlineData("({a})", 3, 3)]
        public void Evaluate_Substitute(string input, double variable, double expected)
        {
            MathExpr expr = input.Substitute("a", variable);

            Assert.Equal(expected, expr.Result);
        }

        [Theory]
        [InlineData("abs -5", 5)]
        [InlineData("abs(-1)", 1)]
        [InlineData("3 max 2", 3)]
        [InlineData("2 x\r\n 5", 10)]
        [InlineData("3 << 2", 12)]
        [InlineData("-3 <> 2", -32)]
        public void Evaluate_CustomOperators(string input, double expected)
        {
            MathExpr expr = input;
            Assert.Equal(expected, expr.Result);
        }

        [Theory]
        [InlineData("abs -5 + {a}", 1, 6)]
        [InlineData("{a} + 2 * abs(-1) / {a}", 1, 3)]
        [InlineData("3 max {a}", 2, 3)]
        [InlineData("2 x\r\n {a}", 5, 10)]
        [InlineData("{a} << {a}", 3, 24)]
        [InlineData("-3 <> {a}", 2, -32)]
        [InlineData("{a}+2", 1, 3)]
        [InlineData("2*{a}+2", 3, 8)]
        [InlineData("2*{a}+2*{a}", 3, 12)]
        [InlineData("({a})", 3, 3)]
        [InlineData("2 * ({a} + 3 + 5)", 1, 18)]
        public void Evaluate_With_Variables(string input, double variable, double expected)
        {
            MathExpr expr = input;
            expr.Substitute("a", variable);

            Assert.Equal(expr.Result, expected);
        }

        [Theory]
        [InlineData("abs -5", 5)]
        [InlineData("abs(-1)", 1)]
        [InlineData("3 max 2", 3)]
        [InlineData("2 x\r\n 5", 10)]
        [InlineData("3 << 2", 12)]
        [InlineData("-3 <> 2", -32)]
        public void Evaluate_CachedOperation_Without_Variables(string input, double expected)
        {
            MathExpr expr = input;

            Assert.Equal(expected, expr.Result);
            Assert.Equal(expected, expr.Result);
        }

        [Theory]
        [InlineData("{a}+2")]
        public void Evaluate_Unassigned_Variable_Exception(string input)
        {
            MathExpr expr = input;

            MathException exception = Assert.Throws<MathException>(() => expr.Result.ToString());
            Assert.Equal(MathException.ErrorCode.UNASSIGNED_VARIABLE, exception.Code);
        }

        [Fact]
        public void Evaluate_Sharing_Context()
        {
            MathExpr expr = "{a} + 1".Substitute("a", 2);
            expr.SetOperator("+", (a, b) => Math.Pow(a, b));

            Assert.Equal(2, expr.Result);

            MathExpr expr2 = "3 + 2".ToMathExpr(expr.Context);
            Assert.Equal(9, expr2.Result);

            double result = "1 + 2 + 3".Eval(expr.Context);
            Assert.Equal(1, result);
        }

        [Fact]
        public void Evaluate_Custom_Context()
        {
            var context = new MathContext();
            context.RegisterBinary("+", (a, b) => Math.Pow(a, b));

            MathExpr expr = new MathExpr("{a} + 1", context).Substitute("a", 2);
            Assert.Equal(2, expr.Result);

            MathExpr expr2 = "3 + 2".ToMathExpr(context);
            Assert.Equal(9, expr2.Result);

            double result = "1 + 2 + 3".Eval(context);
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData("1 + 5", 6)]
        [InlineData("1 + -5", -4)]
        [InlineData("2 * (abs(-5) + 1)", 12)]
        public void Compile(string input, double expected)
        {
            Func<double> fn = input.ToMathExpr().Compile();
            double result = fn();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1 + {a}", new[] { "a" }, new[] { 1d }, 2)]
        public void Compile1(string input, string[] paramsOrder, double[] paramsValues, double expected)
        {
            Func<double, double> fn = input.ToMathExpr().Compile(paramsOrder[0]);
            double result = fn(paramsValues[0]);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("(1 + {a}) * {b}", new[] { "b", "a" }, new[] { 2d, 3d }, 8)]
        [InlineData("(1 + {b}) * {a}", new[] { "b", "a" }, new[] { 2d, 3d }, 9)]
        public void Compile2(string input, string[] paramsOrder, double[] paramsValues, double expected)
        {
            Func<double, double, double> fn = input.ToMathExpr().Compile(paramsOrder[0], paramsOrder[1]);
            double result = fn(paramsValues[0], paramsValues[1]);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Compile1_Throws_Missing_Variable()
        {
            MathException ex = Assert.Throws<MathException>(() => "1 + {a}".ToMathExpr().Compile("b"));

            Assert.Equal(MathException.ErrorCode.UNEXISTING_VARIABLE, ex.Code);
        }
    }
}
