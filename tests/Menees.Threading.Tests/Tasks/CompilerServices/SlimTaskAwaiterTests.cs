namespace Menees.Threading.Tasks.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

[TestClass]
public class SlimTaskAwaiterTests
{
	[TestMethod]
	public void SlimTaskAwaiter_GetResult_Returns_Result()
	{
		var task = new SlimTask<int>(55);
		var awaiter = new SlimTaskAwaiter<int>(task);
		awaiter.GetResult().ShouldBe(55);
	}

	[TestMethod]
	public void SlimTaskAwaiter_IsCompleted_True_For_Completed()
	{
		var task = new SlimTask<int>(1);
		var awaiter = new SlimTaskAwaiter<int>(task);
		awaiter.IsCompleted.ShouldBeTrue();
	}
}
