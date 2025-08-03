using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Menees.Threading.Tasks.CompilerServices;

[TestClass]
public class SlimTaskAwaiterTests
{
	[TestMethod]
	public void SlimTaskAwaiter_GetResult_Returns_Result()
	{
		SlimTask<int> task = new(55);
		SlimTaskAwaiter<int> awaiter = new(task);
		awaiter.GetResult().ShouldBe(55);
	}

	[TestMethod]
	public void SlimTaskAwaiter_IsCompleted_True_For_Completed()
	{
		SlimTask<int> task = new(1);
		SlimTaskAwaiter<int> awaiter = new(task);
		awaiter.IsCompleted.ShouldBeTrue();
	}
}
