using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>Represents a builder for asynchronous methods that returns a <see cref="SlimTask{TResult}"/>.</summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
[StructLayout(LayoutKind.Auto)]
public struct AsyncSlimTaskMethodBuilder<TResult>
{
	/// <summary>Sentinel object used to indicate that the builder completed synchronously and successfully.</summary>
	/// <remarks>
	/// To avoid memory safety issues even in the face of invalid race conditions, we ensure that the type of this object
	/// is valid for the mode in which we're operating.  As such, it's cached on the generic builder per TResult
	/// rather than having one sentinel instance for all types.
	/// </remarks>
	private static readonly Task<TResult> s_syncSuccessSentinel = System.Threading.Tasks.Task.FromResult(default(TResult)!);

	/// <summary>
	/// Used to start the state machine and generate a Task if we need one.
	/// </summary>
	private AsyncTaskMethodBuilder<TResult> _taskBuilder; // Mutable struct! Do not make it readonly!

	/// <summary>
	/// The wrapped task.  If the operation completed synchronously and successfully, this will be a sentinel object
	/// compared by reference identity.</summary>
	private Task<TResult>? _task;

	/// <summary>
	/// The result for this builder if it's completed synchronously, in which case <see cref="_task"/> will be
	/// <see cref="s_syncSuccessSentinel"/>.
	/// </summary>
	private TResult _result;

	private AsyncSlimTaskMethodBuilder(AsyncTaskMethodBuilder<TResult> taskBuilder)
	{
		_taskBuilder = taskBuilder;
		_result = default!;
	}

	/// <summary>Creates a new instance with an assigned task builder.</summary>
	public static AsyncSlimTaskMethodBuilder<TResult> Create()
		=> new(AsyncTaskMethodBuilder<TResult>.Create());

	/// <summary>Gets the SlimTask for this builder.</summary>
	public SlimTask<TResult> Task
	{
		get
		{
			SlimTask<TResult> slimTask;

			if (_task == s_syncSuccessSentinel)
			{
				slimTask = new SlimTask<TResult>(_result);
			}
			else
			{
				// With normal access patterns, _task should always be non-null here: the async method should have
				// either completed synchronously, in which case SetResult would have set _task to a non-null object,
				// or it should be completing asynchronously, in which case AwaitUnsafeOnCompleted would have similarly
				// initialized _task to a state machine object. However, if the type is used manually (not via
				// compiler-generated code) and accesses Task directly, we force it to be initialized.  Things will then
				// "work" but in a degraded mode, as we don't know the TStateMachine type here, and thus we use a
				// normal task object instead.
				Task<TResult>? task = _task ??= _taskBuilder.Task;
				slimTask = new SlimTask<TResult>(task);
			}

			return slimTask;
		}
	}

	/// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
	public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
		where TAwaiter : INotifyCompletion
		where TStateMachine : IAsyncStateMachine
	{
		_taskBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
		_task = _taskBuilder.Task;
	}

	/// <summary>Schedules the state machine to proceed to the next action when the specified awaiter completes.</summary>
	public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
		where TAwaiter : ICriticalNotifyCompletion
		where TStateMachine : IAsyncStateMachine
	{
		_taskBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
		_task = _taskBuilder.Task;
	}

	/// <summary>Marks the SlimTask as failed and binds the specified exception to the SlimTask.</summary>
	public readonly void SetException(Exception exception)
		=> _taskBuilder.SetException(exception);

	/// <summary>Marks the SlimTask as successfully completed.</summary>
	public void SetResult(TResult result)
	{
		if (_task is null)
		{
			_result = result;
			_task = s_syncSuccessSentinel;
		}
		else
		{
			_taskBuilder.SetResult(result);
		}
	}

	/// <summary>Associates the builder with the specified state machine.</summary>
	public readonly void SetStateMachine(IAsyncStateMachine stateMachine)
		=> _taskBuilder.SetStateMachine(stateMachine);

	/// <summary>Begins running the builder with the associated state machine.</summary>
	public readonly void Start<TStateMachine>(ref TStateMachine stateMachine)
		where TStateMachine : IAsyncStateMachine
		=> _taskBuilder.Start(ref stateMachine);
}
