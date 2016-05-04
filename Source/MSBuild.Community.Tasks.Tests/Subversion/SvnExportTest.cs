

using System;
using System.IO;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnExportTest
    /// </summary>
    [TestFixture]
    public class SvnExportTest
    {
        private string testDirectory;

        [OneTimeSetUp]
        public void FixtureInit()
        {
            MockBuild buildEngine = new MockBuild();

            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
        }

        [Test(Description = "Export local repository")]
        public void SvnExportLocal()
        {
            string repoPath = "d:/svn/repo/Test";
            DirectoryInfo dirInfo = new DirectoryInfo(repoPath);
            if (!dirInfo.Exists)
            {
                Assert.Ignore("Repository path '{0}' does not exist", repoPath);
            }

            string exportDir = Path.Combine(testDirectory, @"TestExport");
            if (Directory.Exists(exportDir))
                Directory.Delete(exportDir, true);

            SvnExport export = new SvnExport();
            export.BuildEngine = new MockBuild();

            Assert.IsNotNull(export);

            export.LocalPath = exportDir;
            export.RepositoryPath = "file:///" + repoPath + "/trunk";
            bool result = export.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(export.Revision > 0);
        }

        [Test(Description = "Export remote repository")]
        public void SvnExportRemote()
        {
            string exportDir = Path.Combine(testDirectory, @"MSBuildTasksExport");
            if (Directory.Exists(exportDir))
                Directory.Delete(exportDir, true);

            SvnExport export = new SvnExport();
            export.BuildEngine = new MockBuild();

            Assert.IsNotNull(export);

            export.LocalPath = exportDir;
            export.RepositoryPath =
                "http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk/Source/MSBuild.Community.Tasks.Tests/Subversion";
            export.Username = "guest";
            export.Password = "guest";
            bool result = export.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(export.Revision > 0);
        }

        [Test]
        public void SvnExportRemoteCommand()
        {
            string exportDir = Path.Combine(testDirectory, @"MSBuildTasksExport");
            string remotePath =
                "http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk/Source/MSBuild.Community.Tasks.Tests/Subversion";

            SvnExport export = new SvnExport();

            export.LocalPath = exportDir;
            export.RepositoryPath = remotePath;
            export.Username = "guest";
            export.Password = "guest1";

            string expectedCommand =
                String.Format(
                    "export \"{0}\" \"{1}\" --username guest --password guest1 --non-interactive --no-auth-cache",
                    remotePath, exportDir);
            string actualCommand = TaskUtility.GetToolTaskCommand(export);
            Assert.AreEqual(expectedCommand, actualCommand);
        }
    }
}