using BenchmarkDotNet.Running;

namespace Menees.Threading.Benchmarks;

internal sealed class Program
{
	private static void Main(string[] args)
		=> BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
