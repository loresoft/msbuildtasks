using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.IIS;

namespace MSBuild.Community.Tasks.Tests.IIS
{
	[TestFixture]
	public class WebDirectoryCreateTest
	{
		[Test]
		public void WebDirectoryCreateExecute()
		{
			// Local machine test
			WebDirectoryCreate task = new WebDirectoryCreate();
			task.BuildEngine = new MockBuild();
			task.VirtualDirectoryName = "VirDirTest";
			task.VirtualDirectoryPhysicalPath = @"C:\Inetpub\MSBuildDir";
			Assert.IsTrue(task.Execute(), "Execute Failed!");

			// Remote machine test
			task = new WebDirectoryCreate();
			task.BuildEngine = new MockBuild();
			task.ServerName = "fenway";
			task.VirtualDirectoryName = "VirDirTest";
			task.VirtualDirectoryPhysicalPath = @"C:\Inetpub\MSBuildDir";
			//			task.Username = "testuser";
			//			task.Password = "password";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
