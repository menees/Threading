namespace Menees.Threading.Tasks.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

[TestClass]
public class ConfiguredSlimTaskAwaiterTests
{
	[TestMethod]
	public void ConfiguredSlimTaskAwaiter_GetResult_Returns_Result()
	{
		var task = new SlimTask<int>(77);
		var awaiter = new ConfiguredSlimTaskAwaiter<int>(task, false);
		awaiter.GetResult().ShouldBe(77);
	}

	[TestMethod]
	public void ConfiguredSlimTaskAwaiter_IsCompleted_True_For_Completed()
	{
		var task = new SlimTask<int>(2);
		var awaiter = new ConfiguredSlimTaskAwaiter<int>(task, false);
		awaiter.IsCompleted.ShouldBeTrue();
	}
}
