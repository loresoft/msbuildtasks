

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    [TestFixture]
    public class SvnCommitTest
    {
        [Test]
        public void SvnCommitExecute()
        {
            SvnCommit commit = new SvnCommit();
            commit.Targets = new ITaskItem[] {new TaskItem("a.txt"), new TaskItem("b.txt")};
            commit.Message = "Test";
            string expectedCommand = "commit \"a.txt\" \"b.txt\" --message \"Test\" --non-interactive --no-auth-cache";
            string actualCommand = TaskUtility.GetToolTaskCommand(commit);
            Assert.AreEqual(expectedCommand, actualCommand);
        }
    }
}