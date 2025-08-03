namespace Menees.Threading.Tasks;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Menees.Threading.Tasks.CompilerServices;

/// <summary>
/// Provides a slim task-like value type that wraps a <see cref="Task{TResult}"/> and a <typeparamref name="TResult"/>,
/// only one of which is used, and that does <b>not</b> capture the synchronization context by default.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
/// <remarks>
/// <para>
/// Methods may return an instance of this value type when it's likely that the result of their
/// operations will be available synchronously and when the method is expected to be invoked so
/// frequently that the cost of allocating a new <see cref="Task{TResult}"/> for each call will
/// be prohibitive.
/// </para>
/// <para>
/// This type is useful from library code to avoid having to call <c>ConfigureAwait(false)</c>
/// after every <see langword="await"/> because <see cref="SlimTask{TResult}"/> will not capture
/// the current <see cref="SynchronizationContext"/> by default. If you're using it in an environment
/// where you need to capture the context, then call <c>ConfigureAwait(true)</c> on it.
/// </para>
/// </remarks>
[AsyncMethodBuilder(typeof(AsyncSlimTaskMethodBuilder<>))]
[StructLayout(LayoutKind.Auto)]
public readonly struct SlimTask<TResult> : IEquatable<SlimTask<TResult>>
{
	/// <summary>The result to be used if the operation completed successfully synchronously.</summary>
	private readonly TResult? result;

	/// <summary>null if <see cref="result"/> has the result, otherwise a <see cref="Task{TResult}"/>.</summary>
	private readonly Task<TResult>? task;

	// An instance created with the default ctor (a zero init'd struct) represents a synchronously, successfully
	// completed operation with a result of default(TResult).

	/// <summary>Initialize the <see cref="SlimTask{TResult}"/> with a <typeparamref name="TResult"/> result value.</summary>
	/// <param name="result">The result.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SlimTask(TResult result)
	{
		this.result = result;
		this.task = null;
	}

	/// <summary>Initialize the <see cref="SlimTask{TResult}"/> with a <see cref="Task{TResult}"/> that represents the operation.</summary>
	/// <param name="task">The task.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SlimTask(Task<TResult> task)
	{
		this.task = task ?? throw new ArgumentNullException(nameof(task));
		this.result = default;
	}

	/// <summary>Gets whether the <see cref="SlimTask{TResult}"/> represents a canceled operation.</summary>
	/// <remarks>
	/// If the <see cref="SlimTask{TResult}"/> is backed by a result, this will always return false.
	/// If it's backed by a <see cref="Task"/>, it'll return the value of the task's <see cref="Task.IsCanceled"/> property.
	/// </remarks>
	public bool IsCanceled
		=> this.task != null && this.task.IsCanceled;

	/// <summary>Gets whether the <see cref="SlimTask{TResult}"/> represents a completed operation.</summary>
	public bool IsCompleted
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this.task == null || this.task.IsCompleted;
	}

	/// <summary>Gets whether the <see cref="SlimTask{TResult}"/> represents a successfully completed operation.</summary>
	public bool IsCompletedSuccessfully
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this.task == null ||
#if NET
		this.task.IsCompletedSuccessfully;
#else
		this.task.Status == TaskStatus.RanToCompletion;
#endif
	}

	/// <summary>Gets whether the <see cref="SlimTask{TResult}"/> represents a failed operation.</summary>
	public bool IsFaulted
		=> this.task != null && this.task.IsFaulted;

	/// <summary>Gets the result.</summary>
	public TResult Result
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this.task == null ? this.result! : this.task.GetAwaiter().GetResult();
	}

	/// <summary>Returns a value indicating whether two <see cref="SlimTask{TResult}"/> values are equal.</summary>
	public static bool operator ==(SlimTask<TResult> left, SlimTask<TResult> right) =>
		left.Equals(right);

	/// <summary>Returns a value indicating whether two <see cref="SlimTask{TResult}"/> values are not equal.</summary>
	public static bool operator !=(SlimTask<TResult> left, SlimTask<TResult> right) =>
		!left.Equals(right);

	/// <summary>Returns a value indicating whether this value is equal to a specified <see cref="object"/>.</summary>
	public override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is SlimTask<TResult> task && this.Equals(task);

	/// <summary>Returns the hash code for this instance.</summary>
	public override int GetHashCode() =>
		this.task != null ? this.task.GetHashCode() :
		this.result != null ? this.result.GetHashCode() :
		0;

	/// <summary>Gets a string-representation of this <see cref="SlimTask{TResult}"/>.</summary>
	public override string? ToString()
	{
		string? text = string.Empty;

		if (this.IsCompletedSuccessfully)
		{
			TResult result = this.Result;
			if (result != null)
			{
				text = result.ToString();
			}
		}

		return text;
	}

	/// <summary>
	/// Gets a <see cref="Task{TResult}"/> object to represent this SlimTask.
	/// </summary>
	/// <remarks>
	/// It will either return the wrapped task object if one exists, or it'll
	/// manufacture a new task object to represent the result.
	/// </remarks>
#pragma warning disable CC0061 // Asynchronous method can be terminated with the 'Async' keyword. Like ValueTask's AsTask().
	public Task<TResult> AsTask()
#pragma warning restore CC0061 // Asynchronous method can be terminated with the 'Async' keyword.
	{
		// Return the task if we were constructed from one, otherwise manufacture one.  We don't
		// cache the generated task into _task as it would end up changing both equality comparison
		// and the hash code we generate in GetHashCode.
		return this.task ?? Task.FromResult(this.result!);
	}

	/// <summary>Configures an awaiter for this <see cref="SlimTask{TResult}"/>.</summary>
	/// <param name="continueOnCapturedContext">
	/// true to attempt to marshal the continuation back to the captured context; otherwise, false.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConfiguredSlimTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
		=> new(this, continueOnCapturedContext);

	/// <summary>Returns a value indicating whether this value is equal to a specified <see cref="SlimTask{TResult}"/> value.</summary>
	public bool Equals(SlimTask<TResult> other) =>
		this.task != null || other.task != null ?
			this.task == other.task :
			EqualityComparer<TResult>.Default.Equals(this.result!, other.result!);

	/// <summary>Gets an awaiter for this <see cref="SlimTask{TResult}"/>.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SlimTaskAwaiter<TResult> GetAwaiter() => new(in this);
}
