[![Windows](https://github.com/menees/Threading/actions/workflows/windows.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/windows.yml)
[![Ubuntu](https://github.com/menees/Threading/actions/workflows/ubuntu.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/ubuntu.yml)
[![NuGet](https://img.shields.io/nuget/vpre/Menees.Threading)](https://www.nuget.org/packages/Menees.Threading/)

# Threading
This repo defines some helper types for writing efficient asynchronous code.

## `SlimTask<TResult>`
[`SlimTask<TResult>`](src/Menees.Threading/Tasks/SlimTask.cs) is an awaitable, task-like value type that does **NOT** capture the current synchronization context by default.
It's useful to return from `async` library methods when results are usually available synchronously (similar to [`ValueTask<TResult>`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)) but without having to call [`ConfigureAwait(false)`](https://devblogs.microsoft.com/dotnet/configureawait-faq/#when-should-i-use-configureawait(false)) everywhere.

Since it doesn't capture the synchronization context by default, it doesn't require `ConfigureAwait(false)`. However, it supports `ConfigureAwait(true)` if you need the context, e.g., if your code invokes a passed-in lambda that may require the captured context.

### Examples
``` C#
int value = await GetValueAsync(42); // No need for .ConfigureAwait(false) with SlimTask
...
static async SlimTask<int> GetValueAsync(int value)
{
	int result = value;
	if (result % 100 == 0)
	{
		await Task.Delay(1).ConfigureAwait(false);
	}

	return result;
}
```
For more examples, see [SlimTaskTests.cs](tests/Menees.Threading.Tests/Tasks/SlimTaskTests.cs).

### Benchmarks
The [Menees.Threading.Benchmarks](tests/Menees.Threading.Benchmarks) project uses [BenchmarkDotNet](https://benchmarkdotnet.org/). Its results show that `SlimTask<TResult>` performs about the same as `ValueTask<TResult>` for timing and exactly the same for allocations.

#### .NET Framework 4.8.1
`dotnet run -c Release --framework net48`
``` Text
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Intel Core i7-8086K CPU 4.00GHz (Max: 4.01GHz) (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256
  DefaultJob : .NET Framework 4.8.1 (4.8.9310.0), X64 RyuJIT VectorSize=256

| Method       | Mean     | Error   | StdDev  | Gen0   | Allocated |
|------------- |---------:|--------:|--------:|-------:|----------:|
| UseTask      | 116.2 ns | 1.74 ns | 1.45 ns | 0.0254 |     160 B |
| UseValueTask | 127.3 ns | 1.15 ns | 1.08 ns |      - |         - |
| UseSlimTask  | 120.4 ns | 0.79 ns | 0.74 ns |      - |         - |

| Method           | Mean     | Error     | StdDev    | Gen0   | Allocated |
|----------------- |---------:|----------:|----------:|-------:|----------:|
| ComputeTask      | 2.525 us | 0.0060 us | 0.0057 us | 0.2136 |   1.32 KB |
| ComputeValueTask | 2.517 us | 0.0122 us | 0.0102 us | 0.1869 |   1.17 KB |
| ComputeSlimTask  | 2.475 us | 0.0119 us | 0.0111 us | 0.1869 |   1.17 KB |
```

#### .NET 8
`dotnet run -c Release --framework net8.0`
``` Text
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Intel Core i7-8086K CPU 4.00GHz (Max: 4.01GHz) (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.304
  [Host]     : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.19 (8.0.1925.36514), X64 RyuJIT AVX2

| Method       | Mean     | Error    | StdDev   | Gen0   | Allocated |
|------------- |---------:|---------:|---------:|-------:|----------:|
| UseTask      | 27.18 ns | 0.113 ns | 0.100 ns | 0.0229 |     144 B |
| UseValueTask | 16.79 ns | 0.145 ns | 0.129 ns |      - |         - |
| UseSlimTask  | 18.50 ns | 0.105 ns | 0.098 ns |      - |         - |

| Method           | Mean     | Error   | StdDev  | Gen0   | Allocated |
|----------------- |---------:|--------:|--------:|-------:|----------:|
| ComputeTask      | 651.1 ns | 3.07 ns | 2.88 ns | 0.0544 |     344 B |
| ComputeValueTask | 631.1 ns | 4.02 ns | 3.56 ns | 0.0315 |     200 B |
| ComputeSlimTask  | 662.0 ns | 8.81 ns | 7.81 ns | 0.0315 |     200 B |
```