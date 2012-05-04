using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitVersionTest
    {
        [Test]
        public void Execute()
        {
            GitVersion task = new GitVersion();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute Failed");

            Assert.IsFalse(string.IsNullOrEmpty(task.CommitHash), "Invalid Revision Number");
        }

    }
}
