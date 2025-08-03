namespace Menees.Threading.Tasks.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

[TestClass]
public class AsyncSlimTaskMethodBuilderTests
{
	[TestMethod]
	public void AsyncSlimTaskMethodBuilder_Create_Works()
	{
		var builder = AsyncSlimTaskMethodBuilder<int>.Create();
		builder.ShouldBeOfType<AsyncSlimTaskMethodBuilder<int>>();
	}
}
