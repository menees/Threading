using System.Runtime.CompilerServices;
using System.Threading;

namespace Menees.Threading.Tasks;

internal sealed class TestSynchronizationContext : SynchronizationContext
{
	private readonly List<string>? _calls;

	public TestSynchronizationContext(bool allowCapture)
	{
		if (allowCapture)
		{
			_calls = [];
		}
	}

	public int CallCount => _calls?.Count ?? 0;

	public IDisposable BeginScope()
		=> new ContextScope(this);

	public override void Post(SendOrPostCallback d, object? state)
	{
		ValidateCall();
		base.Post(d, state);
	}

	public override void Send(SendOrPostCallback d, object? state)
	{
		ValidateCall();
		base.Send(d, state);
	}

	private void ValidateCall([CallerMemberName] string? caller = null)
	{
		if (_calls is null)
		{
			throw new InvalidOperationException($"{nameof(TestSynchronizationContext)} should not be captured.");
		}

		if (caller is not null)
		{
			_calls.Add(caller);
		}
	}

	private sealed class ContextScope : IDisposable
	{
		private SynchronizationContext? _previousContext;
		private bool _isDisposed;

		public ContextScope(TestSynchronizationContext context)
		{
			_previousContext = Current;
			SetSynchronizationContext(context);
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				try
				{
					_isDisposed = true;
				}
				finally
				{
					SetSynchronizationContext(_previousContext);
					_previousContext = null;
				}
			}
		}
	}
}
