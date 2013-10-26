using System.IO;
using MSBuild.Community.Tasks.Git;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Git
{
    [TestFixture]
    public class GitCommitsTest
    {
        [Test]
        public void Execute()
        {
            var task = new GitCommits();
            task.BuildEngine = new MockBuild();
            task.ToolPath = @"C:\Program Files (x86)\Git\bin";

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            bool result = task.Execute();
            int commits;
            int.TryParse(task.CommitsCount, out commits);

            Assert.IsTrue(result, "Execute Failed");

            Assert.IsFalse(commits == 0, "Invalid CommitsCount");
        }
    }
}