using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>Provides an awaiter for a <see cref="SlimTask{TResult}"/>.</summary>
/// <remarks>This will never capture the current context. If you need to force a SlimTask
/// to capture the context, then call <see cref="SlimTask{TResult}.ConfigureAwait(bool)"/>
/// with a value of <see langword="true"/>.</remarks>
[StructLayout(LayoutKind.Auto)]
public readonly struct SlimTaskAwaiter<TResult> : ICriticalNotifyCompletion
{
	// NOTE: This means we'll use "continueOnCapturedContext: false", which is the
	// primary way SlimTask<TResult> differs from a non-pooled ValueTask<TResult>.
	private const bool DoNotContinueOnCapturedContext = false;

	/// <summary>The value being awaited.</summary>
	private readonly SlimTask<TResult> _value;

	/// <summary>Initializes the awaiter.</summary>
	/// <param name="value">The value to be awaited.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal SlimTaskAwaiter(in SlimTask<TResult> value) => _value = value;

	/// <summary>Gets whether the <see cref="SlimTask{TResult}"/> has completed.</summary>
	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _value.IsCompleted;
	}

	/// <summary>Gets the result of the SlimTask.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[StackTraceHidden]
	public TResult GetResult() => _value.Result;

	/// <summary>Schedules the continuation action for this SlimTask.</summary>
	public void OnCompleted(Action continuation)
		=> _value.AsTask().ConfigureAwait(DoNotContinueOnCapturedContext).GetAwaiter().OnCompleted(continuation);

	/// <summary>Schedules the continuation action for this SlimTask.</summary>
	public void UnsafeOnCompleted(Action continuation)
		=> _value.AsTask().ConfigureAwait(DoNotContinueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
}