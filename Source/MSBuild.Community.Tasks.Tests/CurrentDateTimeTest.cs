using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
	[TestFixture]
	public class CurrentDateTimeTest
	{
		[Test]
		public void Test()
		{
			// Recycle
			CurrentDateTime task = new CurrentDateTime();
			task.BuildEngine = new MockBuild();
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
