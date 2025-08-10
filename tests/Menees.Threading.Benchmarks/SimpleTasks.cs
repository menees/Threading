using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Menees.Threading.Tasks;

namespace Menees.Threading.Benchmarks;

// Note: Benchmark classes must be unsealed.
[MemoryDiagnoser]
public class SimpleTasks
{
	private const int MagicNumber = 867_5309;

#pragma warning disable CA1822 // Mark members as static. [Benchmark] methods must be instance members.
	[Benchmark]
	public async Task<int> UseTask()
		=> await GetTask().ConfigureAwait(false);

	[Benchmark]
	public async ValueTask<int> UseValueTask()
		=> await GetValueTask().ConfigureAwait(false);

	[Benchmark]
	public async SlimTask<int> UseSlimTask()
		=> await GetSlimTask(); // ConfigureAwait(false) is SlimTask's default.
#pragma warning restore CA1822 // Mark members as static

	private static async Task<int> GetTask()
	{
		await Task.CompletedTask;
		return MagicNumber;
	}

	private static async ValueTask<int> GetValueTask()
	{
		await Task.CompletedTask;
		return MagicNumber;
	}

	private static async SlimTask<int> GetSlimTask()
	{
		await Task.CompletedTask;
		return MagicNumber;
	}
}
