using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>Provides an awaitable type that enables configured awaits on a <see cref="SlimTask{TResult}"/>.</summary>
/// <typeparam name="TResult">The type of the result produced.</typeparam>
/// <remarks>The standard <see cref="SlimTaskAwaiter{TResult}"/> will never capture the current context.
/// However, this type can be used to capture the context if necessary by calling
/// <see cref="SlimTask{TResult}.ConfigureAwait(bool)"/>.</remarks>
[StructLayout(LayoutKind.Auto)]
public readonly struct ConfiguredSlimTaskAwaitable<TResult>
{
	/// <summary>The wrapped <see cref="SlimTask{TResult}"/>.</summary>
	private readonly SlimTask<TResult> _value;

	/// <summary>true to attempt to marshal the continuation back to the original context captured; otherwise, false.</summary>
	private readonly bool _continueOnCapturedContext;

	/// <summary>Initializes the awaitable.</summary>
	/// <param name="value">The wrapped <see cref="SlimTask{TResult}"/>.</param>
	/// <param name="continueOnCapturedContext">The value to pass to ConfigureAwait.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ConfiguredSlimTaskAwaitable(in SlimTask<TResult> value, bool continueOnCapturedContext)
	{
		_value = value;
		_continueOnCapturedContext = continueOnCapturedContext;
	}

	/// <summary>Returns an awaiter for this <see cref="ConfiguredSlimTaskAwaitable{TResult}"/> instance.</summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConfiguredSlimTaskAwaiter<TResult> GetAwaiter()
		=> new(in _value, _continueOnCapturedContext);
}
