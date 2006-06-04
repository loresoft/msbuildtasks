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
		private string mAppPoolName = "AppPoolTest";
		private string mServer = "fenway";
		// private string mWAMUsername = "testuser";
		// private string mWAMPassword = "password";
		
		[Test]
		public void AppPoolCreateLocal()
		{
			// Local machine test
			mServer = "localhost";
			if (!TaskUtility.isIISInstalled(mServer))
			{
				Assert.Ignore(@"IIS was not found on the machine.  IIS is required to run this test.");
			}
			
			AppPoolCreate task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.ApplicationPoolName = mAppPoolName;
			Assert.IsFalse(task.Execute(), "Execute Failed!");
		}
		
		[Test]
		public void AppPoolCreateRemote()
		{
			// Remote machine test
			if (!TaskUtility.isIISInstalled(mServer))
			{
				Assert.Ignore(@"IIS was not found on the machine.  IIS is required to run this test.");
			}
			
			AppPoolCreate task = new AppPoolCreate();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.ApplicationPoolName = mAppPoolName;
			task.PeriodicRestartSchedule = "08:00, 20:00";
			task.AppPoolIdentityType = 3;
			// task.WAMUserName = mWAMUsername
			// task.WAMUserPass = mWAMPassword
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
