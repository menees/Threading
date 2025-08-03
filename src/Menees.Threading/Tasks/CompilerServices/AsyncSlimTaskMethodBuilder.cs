namespace Menees.Threading.Tasks.CompilerServices;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>Represents a builder for asynchronous methods that returns a <see cref="SlimTask{TResult}"/>.</summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly struct AsyncSlimTaskMethodBuilder<TResult>
{
	private readonly AsyncTaskMethodBuilder<TResult> builder;

	/// <summary>Creates a new instance with an assigned task builder.</summary>
	public AsyncSlimTaskMethodBuilder()
	{
		this.builder = AsyncTaskMethodBuilder<TResult>.Create();
	}

	/// <summary>Gets the SlimTask for this builder.</summary>
	public SlimTask<TResult> Task
		=> new(this.builder.Task);

	/// <summary>Creates an instance of the <see cref="AsyncSlimTaskMethodBuilder{TResult}"/> struct.</summary>
	/// <returns>The initialized instance.</returns>
	public static AsyncSlimTaskMethodBuilder<TResult> Create()
		=> new();

	/// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
	/// <typeparam name="TAwaiter">The type of the awaiter.</typeparam>
	/// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
	/// <param name="awaiter">the awaiter</param>
	/// <param name="stateMachine">The state machine.</param>
	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
		where TAwaiter : INotifyCompletion
		where TStateMachine : IAsyncStateMachine
		=> this.builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

	/// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
	/// <typeparam name="TAwaiter">The type of the awaiter.</typeparam>
	/// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
	/// <param name="awaiter">the awaiter</param>
	/// <param name="stateMachine">The state machine.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
		where TAwaiter : ICriticalNotifyCompletion
		where TStateMachine : IAsyncStateMachine
		=> this.builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);

	/// <summary>Marks the SlimTask as failed and binds the specified exception to the SlimTask.</summary>
	/// <param name="exception">The exception to bind to the SlimTask.</param>
	public void SetException(Exception exception)
		=> this.builder.SetException(exception);

	/// <summary>Marks the SlimTask as successfully completed.</summary>
	/// <param name="result">The result to use to complete the SlimTask.</param>
	public void SetResult(TResult result)
		=> this.builder.SetResult(result);

	/// <summary>Associates the builder with the specified state machine.</summary>
	/// <param name="stateMachine">The state machine instance to associate with the builder.</param>
	public void SetStateMachine(IAsyncStateMachine stateMachine)
		=> this.builder.SetStateMachine(stateMachine);

	/// <summary>Begins running the builder with the associated state machine.</summary>
	/// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
	/// <param name="stateMachine">The state machine instance, passed by reference.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Start<TStateMachine>(ref TStateMachine stateMachine)
		where TStateMachine : IAsyncStateMachine
		=> this.builder.Start(ref stateMachine);
}
