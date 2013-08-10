using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitDescribeTest
    {
        [Test]
        public void GitDescribeExecute()
        {
            var task = new GitDescribe();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute Failed");

            Assert.AreNotEqual(task.CommitCount, -1); // -1 designates a soft error.  Only should occur in soft error mode
            Assert.IsFalse(string.IsNullOrEmpty(task.CommitHash), "Invalid Revision Number");
        }

    }
}
