using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Menees.Threading.Tasks.CompilerServices;

[TestClass]
public class ConfiguredSlimTaskAwaiterTests
{
	[TestMethod]
	public void ConfiguredSlimTaskAwaiter_GetResult_Returns_Result()
	{
		SlimTask<int> task = new(77);
		ConfiguredSlimTaskAwaiter<int> awaiter = new(task, false);
		awaiter.GetResult().ShouldBe(77);
	}

	[TestMethod]
	public void ConfiguredSlimTaskAwaiter_IsCompleted_True_For_Completed()
	{
		SlimTask<int> task = new(2);
		ConfiguredSlimTaskAwaiter<int> awaiter = new(task, false);
		awaiter.IsCompleted.ShouldBeTrue();
	}
}
