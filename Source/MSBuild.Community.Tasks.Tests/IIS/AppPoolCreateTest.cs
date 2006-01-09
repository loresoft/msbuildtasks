// $Id$

using System;
using System.Text;
using MSBuild.Community.Tasks.IIS;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class AppPoolCreateTest
	{
		[Test]
		public void AppPoolCreateExecute()
		{
			// Local machine test on Win XP Pro - IIS 5 (should fail)
			AppPoolCreate task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.ApplicationPoolName = "AppPoolTest";
			Assert.IsFalse(task.Execute(), "Execute Failed!");

			// Remote machine test - Windows Server 2003 - IIS 6 (should pass)
			task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.ServerName = "fenway";
			task.ApplicationPoolName = "AppPoolTest";
			task.PeriodicRestartSchedule = "08:00, 20:00";
			task.AppPoolIdentityType = 3;
			//			task.WAMUserName = "testuser";
			//			task.WAMUserPass = "password";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
