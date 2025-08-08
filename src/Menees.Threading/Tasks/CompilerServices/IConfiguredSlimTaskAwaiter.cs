using System.Threading.Tasks;

namespace Menees.Threading.Tasks.CompilerServices;

/// <summary>
/// Internal interface used to enable extracting the Task from arbitrary configured <see cref="SlimTask{TResult}"/> awaiters.
/// </summary>
internal interface IConfiguredSlimTaskAwaiter
{
	(Task task, bool continueOnCapturedContext) GetTask();
}
