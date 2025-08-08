using System.Threading.Tasks;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>
/// Internal interface used to enable extracting the Task from arbitrary <see cref="SlimTask{TResult}"/> awaiters.
/// </summary>
internal interface ISlimTaskAwaiter
{
	Task GetTask();
}
