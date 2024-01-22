using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace StringMath.Benchmarks
{
    [MemoryDiagnoser(true)]
    [SimpleJob(RuntimeMoniker.Net80, warmupCount: 0, iterationCount: 1, launchCount: 1)]
    public class Benchmarks
    {
        [Benchmark]
        public void Tokenize()
        {
            var tokenizer = new Tokenizer("1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}");

            Token token;

            do
            {
                token = tokenizer.ReadToken();
            }
            while (token.Type != TokenType.EndOfCode);
        }

        [Benchmark]
        public void Parse()
        {
            var tokenizer = new Tokenizer("1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}");
            var parser = new Parser(tokenizer, MathContext.Default);
            _ = parser.Parse();
        }

        [Benchmark]
        public double Evaluate()
        {
            return "1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}".ToMathExpr().Substitute("b", 12989d).Substitute("ahghghh", 12345d).Result;
        }

        [Benchmark]
        public double Compile()
        {
            var fn = "1.23235456576878798 - ((3 + {b}) max .1) ^ sqrt(-999 / 2 * 3 max 5) + !5 - 0.00000000002 / {ahghghh}".ToMathExpr().Substitute("b", 12989d).Substitute("ahghghh", 12345d).Compile("ahghghh");
            return fn(12345d);
        }
    }
}
