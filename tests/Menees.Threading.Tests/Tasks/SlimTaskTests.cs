namespace Menees.Threading.Tasks;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

[TestClass]
public class SlimTaskTests
{
	[TestMethod]
	public void SlimTask_Result_Constructor_Works()
	{
		var task = new SlimTask<int>(42);
		task.IsCompleted.ShouldBeTrue();
		task.IsCompletedSuccessfully.ShouldBeTrue();
		task.Result.ShouldBe(42);
		task.IsFaulted.ShouldBeFalse();
		task.IsCanceled.ShouldBeFalse();
	}

	[TestMethod]
	public void SlimTask_Task_Constructor_Works()
	{
		var t = Task.FromResult(99);
		var task = new SlimTask<int>(t);
		task.IsCompleted.ShouldBeTrue();
		task.IsCompletedSuccessfully.ShouldBeTrue();
		task.Result.ShouldBe(99);
	}

	[TestMethod]
	public void SlimTask_Equality_Works()
	{
		var a = new SlimTask<int>(5);
		var b = new SlimTask<int>(5);
		(a == b).ShouldBeTrue();
		(a != b).ShouldBeFalse();
		a.Equals(b).ShouldBeTrue();
		a.Equals((object)b).ShouldBeTrue();
	}

	[TestMethod]
	public void SlimTask_AsTask_Returns_Task()
	{
		var task = new SlimTask<int>(7);
		var t = task.AsTask();
		t.Result.ShouldBe(7);
	}

	[TestMethod]
	public void SlimTask_ToString_Returns_Result_String()
	{
		var task = new SlimTask<int>(123);
		task.ToString().ShouldBe("123");
	}
}
