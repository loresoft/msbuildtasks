using System;
using System.IO;
using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitClientTest
    {
        [Test]
        public void Execute()
        {
            var task = new GitClient();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";


            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            task.Command = "symbolic-ref";
            task.Arguments = "HEAD";

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute Failed");

            Assert.IsTrue(task.ConsoleOutput.Length > 0);
        }

    }
}