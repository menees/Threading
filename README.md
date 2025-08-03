[![Windows](https://github.com/menees/Threading/actions/workflows/windows.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/windows.yml)
[![Ubuntu](https://github.com/menees/Threading/actions/workflows/ubuntu.yml/badge.svg)](https://github.com/menees/Threading/actions/workflows/ubuntu.yml)
[![NuGet](https://img.shields.io/nuget/vpre/Menees.Threading)](https://www.nuget.org/packages/Menees.Threading/)

# Threading
This repo adds a [`SlimTask<TResult>`](src/Menees.Threading/Tasks/SlimTask.cs) task-like value type that does **NOT** capture the current synchronization context by default.
It's useful in library code that would like to return [`ValueTask<TResult>`](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1) but without having to call [`ConfigureAwait(false)`](https://devblogs.microsoft.com/dotnet/configureawait-faq/#when-should-i-use-configureawait(false)) everywhere.
