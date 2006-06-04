// $Id$

using System;
using System.Text;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.IIS;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class WebDirectoryDeleteTest
	{
		private string mVirtualDirectoryName = "VirDirTest";
		private string mServer = "fenway";
		private string mWAMUsername = "testuser";
		private string mWAMPassword = "password";

		[Test]
		public void WebDirectoryDeleteLocal()
		{
			// Local machine test
			mServer = "localhost";
			if (!TaskUtility.isIISInstalled(mServer))
			{
				Assert.Ignore(@"IIS was not found on the machine.  IIS is required to run this test.");
			}
			
			WebDirectoryDelete task = new WebDirectoryDelete();
			task.BuildEngine = new MockBuild();
			task.VirtualDirectoryName = mVirtualDirectoryName;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
		
		[Test]
		public void WebDirectoryDeleteRemote()
		{
			// Remote machine test
			if (!TaskUtility.isIISInstalled(mServer))
			{
				Assert.Ignore(@"IIS was not found on the machine.  IIS is required to run this test.");
			}
			
			WebDirectoryDelete task = new WebDirectoryDelete();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.VirtualDirectoryName = mVirtualDirectoryName;
			//			task.Username = mWAMUsername;
			//			task.Password = mWAMPassword;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
