[![Windows](https://github.com/menees/Threading/actions/workflows/windows.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/windows.yml)
[![Ubuntu](https://github.com/menees/Threading/actions/workflows/ubuntu.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/ubuntu.yml)
[![NuGet](https://img.shields.io/nuget/vpre/Menees.Threading)](https://www.nuget.org/packages/Menees.Threading/)

# Threading
This repo defines an awaitable, task-like [`SlimTask<TResult>`](src/Menees.Threading/Tasks/SlimTask.cs) value type that does **NOT** capture the current synchronization context by default.
It's useful in library code that would like to return a value type (similar to [`ValueTask<TResult>`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1)) but without having to call [`ConfigureAwait(false)`](https://devblogs.microsoft.com/dotnet/configureawait-faq/#when-should-i-use-configureawait(false)) everywhere.

Because it doesn't capture the synchronization context by default, it doesn't require `ConfigureAwait(false)` everywhere. However, it supports `ConfigureAwait(true)` if you need the context, e.g., if your library invokes a passed-in lambda that may require the captured context.
