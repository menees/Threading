using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Menees.Threading.Tasks.CompilerServices;

[TestClass]
public class AsyncSlimTaskMethodBuilderTests
{
	[TestMethod]
	public void AsyncSlimTaskMethodBuilder_Create_Works()
	{
		AsyncSlimTaskMethodBuilder<int> builder = AsyncSlimTaskMethodBuilder<int>.Create();
		builder.ShouldBeOfType<AsyncSlimTaskMethodBuilder<int>>();
	}
}
