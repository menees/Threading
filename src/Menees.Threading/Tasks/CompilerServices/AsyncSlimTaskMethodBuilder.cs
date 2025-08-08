using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>Represents a builder for asynchronous methods that returns a <see cref="SlimTask{TResult}"/>.</summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
[StructLayout(LayoutKind.Auto)]
public struct AsyncSlimTaskMethodBuilder<TResult>
{
	/// <summary>The <see cref="AsyncTaskMethodBuilder{TResult}"/> to which most operations are delegated.</summary>
	private AsyncTaskMethodBuilder<TResult> _methodBuilder; // Mutable struct! Do not make it readonly!

	/// <summary>The result for this builder, if it's completed before any awaits occur.</summary>
	private TResult _result;

	/// <summary>true if <see cref="_result"/> contains the synchronous result for the async method; otherwise, false.</summary>
	private bool _haveResult;

	/// <summary>true if the builder should be used for setting/getting the result; otherwise, false.</summary>
	private bool _useBuilder;

	/// <summary>Creates a new instance with an assigned task builder.</summary>
	public static AsyncSlimTaskMethodBuilder<TResult> Create()
		=> new() { _methodBuilder = AsyncTaskMethodBuilder<TResult>.Create() };

	/// <summary>Gets the task for this builder.</summary>
	public SlimTask<TResult> Task
	{
		get
		{
			SlimTask<TResult> slimTask;

			if (_haveResult)
			{
				slimTask = new SlimTask<TResult>(_result);
			}
			else
			{
				_useBuilder = true;
				slimTask = new SlimTask<TResult>(_methodBuilder.Task);
			}

			return slimTask;
		}
	}

	/// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
	/// <typeparam name="TAwaiter">The type of the awaiter.</typeparam>
	/// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
	/// <param name="awaiter">the awaiter</param>
	/// <param name="stateMachine">The state machine.</param>
	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
		where TAwaiter : INotifyCompletion
		where TStateMachine : IAsyncStateMachine
	{
		_useBuilder = true;
		_methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
	}

	/// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
	/// <typeparam name="TAwaiter">The type of the awaiter.</typeparam>
	/// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
	/// <param name="awaiter">the awaiter</param>
	/// <param name="stateMachine">The state machine.</param>
	[SecuritySafeCritical]
	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
		where TAwaiter : ICriticalNotifyCompletion
		where TStateMachine : IAsyncStateMachine
	{
		_useBuilder = true;
		_methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
	}

	/// <summary>Marks the SlimTask as failed and binds the specified exception to it.</summary>
	/// <param name="exception">The exception to bind to the task.</param>
	public void SetException(Exception exception) => _methodBuilder.SetException(exception);

	/// <summary>Marks the SlimTask as successfully completed.</summary>
	/// <param name="result">The value to use to complete the task.</param>
	public void SetResult(TResult result)
	{
		if (_useBuilder)
		{
			_methodBuilder.SetResult(result);
		}
		else
		{
			_result = result;
			_haveResult = true;
		}
	}

	/// <summary>Associates the builder with the specified state machine.</summary>
	/// <param name="stateMachine">The state machine instance to associate with the builder.</param>
	public void SetStateMachine(IAsyncStateMachine stateMachine)
		=> _methodBuilder.SetStateMachine(stateMachine);

	/// <summary>Begins running the builder with the associated state machine.</summary>
	/// <typeparam name="TStateMachine">The type of the state machine.</typeparam>
	/// <param name="stateMachine">The state machine instance, passed by reference.</param>
	public void Start<TStateMachine>(ref TStateMachine stateMachine)
		where TStateMachine : IAsyncStateMachine
		=> _methodBuilder.Start(ref stateMachine); // will provide the right ExecutionContext semantics
}
