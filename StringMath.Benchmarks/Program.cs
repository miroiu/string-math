using BenchmarkDotNet.Running;

namespace StringMath.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<TokenizerBenchmarks>();
        }
    }
}