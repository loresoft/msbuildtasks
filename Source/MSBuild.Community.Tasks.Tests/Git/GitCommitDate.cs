using System;
using System.IO;
using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitCommitDateTest
    {
        [Test]
        public void GitCommitDate()
        {
			string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            var task = new GitCommitDate
            {
	            BuildEngine = new MockBuild(),
	            LocalPath = Path.Combine(prjRootPath, @"Source")
            };
	        bool result = task.Execute();
            Console.Write(task.CommitDate);
            Assert.IsTrue(result, "Execute Failed");
        }
    }
}