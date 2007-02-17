using System;
using System.IO;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    [TestFixture]
    public class SvnInfoTests
    {
        /// <summary>
        /// This test is coupled with the repository. It may require network connectivity
        /// in order to pass (haven't yet tested). Also, if this site is ever rehosted
        /// such that the URL changes then this test will need to be updated. The upside here
        /// is that we actually run "svn info" and test some if it's results.
        /// </summary>
        [Test]
        public void TestInfoReturnValues()
        {
            SvnInfo info = new SvnInfo();
            info.LocalPath = Path.Combine(TaskUtility.getProjectRootDirectory(true), "Source");
            info.BuildEngine = new MockBuild();
            Assert.IsTrue(info.Execute());

            string val = info.RepositoryPath;

            // "http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk"
            // could also be svn://
            Assert.AreEqual(0, val.IndexOf("http://"));
            Assert.AreEqual(NodeKind.dir.ToString(), info.NodeKind);
            Assert.AreEqual("http://msbuildtasks.tigris.org/svn/msbuildtasks", info.RepositoryRoot);
            Assert.AreNotEqual(Guid.Empty, info.RepositoryUuid);
        }

        [Test]
        public void TestInfoCommand()
        {
            SvnInfo info = new SvnInfo();
            string localPath = Path.Combine(TaskUtility.getProjectRootDirectory(true), "Source");
            info.LocalPath = localPath;

            string expectedCommand = String.Format("info \"{0}\" --xml --non-interactive --no-auth-cache", localPath);
            string actualCommand = TaskUtility.GetToolTaskCommand(info);
            Assert.AreEqual(expectedCommand, actualCommand);
        }
    }
}