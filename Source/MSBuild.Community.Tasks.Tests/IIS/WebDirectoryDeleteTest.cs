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
		[Test]
		public void WebDirectoryDeleteExecute()
		{
			// Local machine test
			WebDirectoryDelete task = new WebDirectoryDelete();
			task.BuildEngine = new MockBuild();
			task.VirtualDirectoryName = "VirDirTest";
			Assert.IsTrue(task.Execute(), "Execute Failed!");

			// Remote machine test
			task = new WebDirectoryDelete();
			task.BuildEngine = new MockBuild();
			task.ServerName = "fenway";
			task.VirtualDirectoryName = "VirDirTest";
			//			task.Username = "testuser";
			//			task.Password = "password";
			Assert.IsTrue(task.Execute(), "Execute Failed!");
		}
	}
}
