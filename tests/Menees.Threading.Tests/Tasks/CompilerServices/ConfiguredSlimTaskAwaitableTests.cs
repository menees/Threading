using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Menees.Threading.Tasks.CompilerServices;

[TestClass]
public class ConfiguredSlimTaskAwaitableTests
{
	[TestMethod]
	public void ConfiguredSlimTaskAwaitable_GetAwaiter_Works()
	{
		SlimTask<int> task = new(10);
		ConfiguredSlimTaskAwaitable<int> awaitable = new(task, true);
		ConfiguredSlimTaskAwaiter<int> awaiter = awaitable.GetAwaiter();
		awaiter.IsCompleted.ShouldBeTrue();
		awaiter.GetResult().ShouldBe(10);
	}
}
