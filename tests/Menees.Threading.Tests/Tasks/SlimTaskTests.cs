using System.Diagnostics;
using System.Threading;
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

		task = new SlimTask<int>(Task.FromResult(321));
		task.ToString().ShouldBe("321");

		TaskCompletionSource<int> tcs = new();
		tcs.TrySetCanceled();
		task = new SlimTask<int>(tcs.Task);
		task.ToString().ShouldBeEmpty();
	}

	[TestMethod]
	public async Task SlimTask_Is_Awaitable_Synchronously()
	{
		string thought = await ThinkDeepThoughts(synchronous: true);
		Debug.WriteLine(thought);
	}

	[TestMethod]
	public async Task SlimTask_Is_Awaitable_Asynchronously()
	{
		string thought = await ThinkDeepThoughts();
		Debug.WriteLine(thought);
	}

	[TestMethod]
	public async Task SlimTask_Does_Not_Capture_SynchronizationContext_Current_By_Default()
	{
		TestSynchronizationContext context = new(allowCapture: false);
		using (context.BeginScope())
		{
			string thought = await ThinkDeepThoughts();
			Debug.WriteLine(thought);
			context.CallCount.ShouldBe(0);
		}

		context.CallCount.ShouldBe(0);
	}

	[TestMethod]
	public async Task SlimTask_Does_Not_Capture_SynchronizationContext_Current_With_ConfigureAwait_False()
	{
		TestSynchronizationContext context = new(allowCapture: false);
		using (context.BeginScope())
		{
			string thought = await ThinkDeepThoughts().ConfigureAwait(false);
			Debug.WriteLine(thought);
			context.CallCount.ShouldBe(0);
		}

		context.CallCount.ShouldBe(0);
	}

	[TestMethod]
	public async Task SlimTask_Can_Capture_SynchronizationContext_Current_With_ConfigureAwait_True()
	{
		TestSynchronizationContext context = new(allowCapture: true);
		using (context.BeginScope())
		{
			string thought = await ThinkDeepThoughts().ConfigureAwait(true);
			Debug.WriteLine(thought);
			context.CallCount.ShouldBe(1);
		}

		context.CallCount.ShouldBe(1);
	}

	[TestMethod]
	public async ValueTask SlimTask_Can_Be_Used_With_ValueTask()
		=> await ThinkDeepThoughts();

	[TestMethod]
	public async Task SlimTask_Is_Useful_For_Mostly_Synchronous_Results()
	{
		const int RangeMax = 1000;
		const int Modulus = 100;

		int synchronousCompletions = 0;
		foreach (int index in Enumerable.Range(0, RangeMax))
		{
			SlimTask<int> slimTask = GetValueAsync(index);
			if (slimTask.IsCompletedSuccessfully)
			{
				synchronousCompletions++;
			}

			int value = await slimTask;
			value.ShouldBe(index);

			// SlimTask (unlike ValueTask) is safe to await multiple times
			// since it doesn't support IValueTaskSource pooling.
			(await slimTask).ShouldBe(index);
			slimTask.Result.ShouldBe(index);
		}

		synchronousCompletions.ShouldBe(RangeMax - (RangeMax / Modulus));

		static async SlimTask<int> GetValueAsync(int value)
		{
			int result = value;
			if (result % Modulus == 0)
			{
				await Task.Delay(1).ConfigureAwait(false);
			}

			return result;
		}
	}

	private static async SlimTask<string> ThinkDeepThoughts(bool synchronous = false)
	{
		if (synchronous)
		{
			await Task.CompletedTask;
		}
		else
		{
			await Task.Delay(1).ConfigureAwait(false);
		}

		return "Deep";
	}
}
