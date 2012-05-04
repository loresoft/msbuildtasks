

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
		private string mVirtualDirectoryNameHeader = "ZVirDirTestHeader";
		private string mVirtualDirectoryPhysicalPath = @"C:\Inetpub\MSBuildDir";
		private string mRemoteServer = "fenway";
		private string mLocalHostHeader = "";
		private string mRemoteHostHeader = ""; //insert host header name here - try IP address too

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
			string mServer = mRemoteServer;
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

		[Test]
		public void WebDirectoryCreateLocalWithHostHeader()
		{
			// Local machine test
			if (!TaskUtility.IsMinimumIISVersionInstalled("localhost", 5, 0))
			{
				Assert.Ignore(@"IIS 5.0 was not found on the machine.  IIS 5.0 is required to run this test.");
			}

			WebDirectoryCreate task = new WebDirectoryCreate();
			task.BuildEngine = new MockBuild();
			task.VirtualDirectoryName = mVirtualDirectoryNameHeader;
			task.VirtualDirectoryPhysicalPath = mVirtualDirectoryPhysicalPath;
			task.HostHeaderName = mLocalHostHeader;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

		[Test]
		public void WebDirectoryCreateRemoteWithHostHeader()
		{
			string mServer = mRemoteServer;
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
			task.VirtualDirectoryName = mVirtualDirectoryNameHeader;
			task.VirtualDirectoryPhysicalPath = mVirtualDirectoryPhysicalPath;
			task.HostHeaderName = mRemoteHostHeader;
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}

	}
}
