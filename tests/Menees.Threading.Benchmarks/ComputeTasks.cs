using System.Numerics;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Menees.Threading.Tasks;

namespace Menees.Threading.Benchmarks;

[MemoryDiagnoser]
public class ComputeTasks // Must be unsealed
{
#pragma warning disable CA1822 // Mark members as static. [Benchmark] methods must be instance members.
	[Benchmark]
	public async Task<string> ComputeTask()
		=> await GetTask().ConfigureAwait(false);

	[Benchmark]
	public async ValueTask<string> ComputeValueTask()
		=> await GetValueTask().ConfigureAwait(false);

	[Benchmark]
	public async SlimTask<string> ComputeSlimTask()
		=> await GetSlimTask(); // ConfigureAwait(false) is SlimTask's default.
#pragma warning restore CA1822 // Mark members as static

	private static async Task<string> GetTask()
	{
		await Task.CompletedTask;
		return Compute();
	}

	private static async ValueTask<string> GetValueTask()
	{
		await Task.CompletedTask;
		return Compute();
	}

	private static async SlimTask<string> GetSlimTask()
	{
		await Task.CompletedTask;
		return Compute();
	}

	private static string Compute()
	{
#pragma warning disable MEN010 // Avoid magic numbers.
		// Force complex roots by making b^2-4ac < 0.
		Complex a = 157;
		Complex b = 27;
		Complex c = 291;

		Complex squareRoot = Complex.Sqrt((b * b) - (4 * a * c));
		Complex divisor = 2 * a;

		Complex root1 = (-b + squareRoot) / divisor;
		Complex root2 = (-b - squareRoot) / divisor;
		string result = $"{root1} {root2}";
		return result ;
#pragma warning restore MEN010 // Avoid magic numbers
	}
}
