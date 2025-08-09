[![Windows](https://github.com/menees/Threading/actions/workflows/windows.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/windows.yml)
[![Ubuntu](https://github.com/menees/Threading/actions/workflows/ubuntu.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/ubuntu.yml)
[![NuGet](https://img.shields.io/nuget/vpre/Menees.Threading)](https://www.nuget.org/packages/Menees.Threading/)

# Threading
This repo defines some helper types for writing efficient asynchronous code.

## `SlimTask<TResult>`
[`SlimTask<TResult>`](src/Menees.Threading/Tasks/SlimTask.cs) is an awaitable, task-like value type that does **NOT** capture the current synchronization context by default.
It's useful in library code that would like to return a value type in cases where results are mostly available synchronously (similar to [`ValueTask<TResult>`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)) but without having to call [`ConfigureAwait(false)`](https://devblogs.microsoft.com/dotnet/configureawait-faq/#when-should-i-use-configureawait(false)) everywhere.

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