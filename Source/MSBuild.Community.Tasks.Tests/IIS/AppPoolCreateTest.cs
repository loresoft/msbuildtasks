using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.IIS;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class AppPoolCreateTest
	{
		[Test]
		public void AppPoolCreateExecute()
		{
			// Local machine test
			AppPoolCreate task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.AppPoolName = "AppPoolTest";
			Assert.IsFalse(task.Execute(), "Execute Failed!");

			// Remote machine test
			task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.ServerName = "fenway";
			task.AppPoolName = "AppPoolTest";
			task.PeriodicRestartSchedule = "08:00, 20:00";
			task.AppPoolIdentityType = 3;
			task.WAMUserName = "testuser";
			task.WAMUserPass = "password";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
