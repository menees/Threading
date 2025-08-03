using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Menees.Threading.Tasks;

[TestClass]
public class SlimTaskTests
{
	[TestMethod]
	public void SlimTask_Result_Constructor_Works()
	{
		SlimTask<int> task = new(42);
		task.IsCompleted.ShouldBeTrue();
		task.IsCompletedSuccessfully.ShouldBeTrue();
		task.Result.ShouldBe(42);
		task.IsFaulted.ShouldBeFalse();
		task.IsCanceled.ShouldBeFalse();
	}

	[TestMethod]
	public void SlimTask_Task_Constructor_Works()
	{
		Task<int> t = Task.FromResult(99);
		SlimTask<int> task = new(t);
		task.IsCompleted.ShouldBeTrue();
		task.IsCompletedSuccessfully.ShouldBeTrue();
		task.Result.ShouldBe(99);
	}

	[TestMethod]
	public void SlimTask_Equality_Works()
	{
		SlimTask<int> a = new(5);
		SlimTask<int> b = new(5);
		(a == b).ShouldBeTrue();
		(a != b).ShouldBeFalse();
		a.Equals(b).ShouldBeTrue();
		a.Equals((object)b).ShouldBeTrue();
	}

	[TestMethod]
	public void SlimTask_AsTask_Returns_Task()
	{
		SlimTask<int> task = new(7);
		Task<int> t = task.AsTask();
		t.Result.ShouldBe(7);
	}

	[TestMethod]
	public void SlimTask_ToString_Returns_Result_String()
	{
		SlimTask<int> task = new(123);
		task.ToString().ShouldBe("123");
	}
}
