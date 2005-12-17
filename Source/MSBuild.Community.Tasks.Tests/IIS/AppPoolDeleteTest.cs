using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.IIS;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class AppPoolDeleteTest
	{
		[Test]
		public void AppPoolDeleteExecute()
		{
			// Local machine test on Win XP Pro - IIS 5 (should fail)
			AppPoolDelete task = new AppPoolDelete();
			task.BuildEngine = new MockBuild();
			task.AppPoolName = "AppPoolTest";
			Assert.IsFalse(task.Execute(), "Execute Failed!");

			// Remote machine test - Windows Server 2003 - IIS 6 (should pass)
			task = new AppPoolDelete();
			task.BuildEngine = new MockBuild();
			task.ServerName = "fenway";
			task.AppPoolName = "AppPoolTest";
			//			task.Username = "testuser";
			//			task.Password = "password";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
