

using System;
using System.Text;
using MSBuild.Community.Tasks.IIS;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class AppPoolDeleteTest
	{
		private string mAppPoolName = "AppPoolTest";
		private string mServer = "fenway";
		// private string mWAMUsername = "testuser";
		// private string mWAMPassword = "password";
		
		[Test]
		public void AppPoolDeleteLocal()
		{
			// Local machine test
			mServer = "localhost";
            if (!TaskUtility.IsMinimumIISVersionInstalled(mServer, 6, 0))
			{
				Assert.Ignore(@"IIS 6.0 was not found on the machine.  IIS 6.0 is required to run this test.");
			}
			
			AppPoolDelete task = new AppPoolDelete();
			task.BuildEngine = new MockBuild();
			task.ApplicationPoolName = mAppPoolName;
			Assert.IsFalse(task.Execute(), "Execute Failed!");
		}
		
		[Test]
		public void AppPoolDeleteRemote()
		{
			// Remote machine test
            if (!TaskUtility.IsMinimumIISVersionInstalled(mServer, 6, 0))
			{
				Assert.Ignore(@"IIS 6.0 was not found on the machine.  IIS 6.0 is required to run this test.");
			}
			
			AppPoolDelete task = new AppPoolDelete();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.ApplicationPoolName = mAppPoolName;
			// task.Username = mWAMUsername;
			// task.Password = mWAMPassword;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
