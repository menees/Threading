using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>Provides a configurable awaiter for a <see cref="ConfiguredSlimTaskAwaitable{TResult}"/>.</summary>
/// <remarks>Note: The similar <see cref="SlimTaskAwaiter{TResult}"/> is not configurable and never captures the context.</remarks>
[StructLayout(LayoutKind.Auto)]
public readonly struct ConfiguredSlimTaskAwaiter<TResult> : ICriticalNotifyCompletion, IConfiguredSlimTaskAwaiter
{
	/// <summary>The value being awaited.</summary>
	private readonly SlimTask<TResult> _value;

	/// <summary>The value to pass to ConfigureAwait.</summary>
	private readonly bool _continueOnCapturedContext;

	/// <summary>Initializes the awaiter.</summary>
	/// <param name="value">The value to be awaited.</param>
	/// <param name="continueOnCapturedContext">The value to pass to ConfigureAwait.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ConfiguredSlimTaskAwaiter(in SlimTask<TResult> value, bool continueOnCapturedContext)
	{
		// TODO: Look for any "in" parameters that are mutable structs. [Bill, 8/7/2025]
		_value = value;
		_continueOnCapturedContext = continueOnCapturedContext;
	}

	/// <summary>Gets whether the <see cref="ConfiguredSlimTaskAwaitable{TResult}"/> has completed.</summary>
	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _value.IsCompleted;
	}

	/// <summary>Gets the result of the SlimTask.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[StackTraceHidden]
	public TResult GetResult()
		=> _value._task == null
			? _value._result!
			: _value._task.GetAwaiter().GetResult();

	/// <summary>Schedules the continuation action for the <see cref="ConfiguredSlimTaskAwaitable{TResult}"/>.</summary>
	public void OnCompleted(Action continuation)
		=> _value.AsTask().ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);

	/// <summary>Schedules the continuation action for the <see cref="ConfiguredSlimTaskAwaitable{TResult}"/>.</summary>
	public void UnsafeOnCompleted(Action continuation)
		=> _value.AsTask().ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);

	/// <summary>Gets the task underlying <see cref="_value"/>.</summary>
	internal Task<TResult> AsTask() => _value.AsTask();

	/// <summary>Gets the task underlying the incomplete <see cref="_value"/>.</summary>
	/// <remarks>This method is used when awaiting and IsCompleted returned false; thus we expect the value task to be wrapping a non-null task.</remarks>
	(Task task, bool continueOnCapturedContext) IConfiguredSlimTaskAwaiter.GetTask() => (_value.AsTaskExpectNonNull(), _continueOnCapturedContext);
}
