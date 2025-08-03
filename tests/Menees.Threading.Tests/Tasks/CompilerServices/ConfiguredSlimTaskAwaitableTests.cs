namespace Menees.Threading.Tasks.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

[TestClass]
public class ConfiguredSlimTaskAwaitableTests
{
	[TestMethod]
	public void ConfiguredSlimTaskAwaitable_GetAwaiter_Works()
	{
		var task = new SlimTask<int>(10);
		var awaitable = new ConfiguredSlimTaskAwaitable<int>(task, true);
		var awaiter = awaitable.GetAwaiter();
		awaiter.IsCompleted.ShouldBeTrue();
		awaiter.GetResult().ShouldBe(10);
	}
}
