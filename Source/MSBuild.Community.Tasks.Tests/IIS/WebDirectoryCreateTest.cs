// $Id$

using System;
using System.Text;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.IIS;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class WebDirectoryCreateTest
	{
		private string mVirtualDirectoryName = "VirDirTest";
		private string mVirtualDirectoryPhysicalPath = @"C:\Inetpub\MSBuildDir";
		
		[Test]
		public void WebDirectoryCreateLocal()
		{
			// Local machine test
            if (!TaskUtility.IsMinimumIISVersionInstalled("localhost", 5, 0))
			{
				Assert.Ignore(@"IIS 5.0 was not found on the machine.  IIS 5.0 is required to run this test.");
			}
			
			WebDirectoryCreate task = new WebDirectoryCreate();
			task.BuildEngine = new MockBuild();
			task.VirtualDirectoryName = mVirtualDirectoryName;
			task.VirtualDirectoryPhysicalPath = mVirtualDirectoryPhysicalPath;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
		
		[Test]
		public void WebDirectoryCreateRemote()
		{
		    string mServer = "fenway";
            if (!TaskUtility.IsAdminOnRemoteMachine(mServer))
            {
                Assert.Ignore(String.Format("Unable to connect as administrator to {0}", mServer));
            }
			// Remote machine test
            if (!TaskUtility.IsMinimumIISVersionInstalled(mServer, 5, 0))
			{
				Assert.Ignore(@"IIS 5.0 was not found on the machine.  IIS 5.0 is required to run this test.");
			}
			
			WebDirectoryCreate task = new WebDirectoryCreate();
			task.BuildEngine = new MockBuild();
			task.ServerName = mServer;
			task.VirtualDirectoryName = mVirtualDirectoryName;
			task.VirtualDirectoryPhysicalPath = mVirtualDirectoryPhysicalPath;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
