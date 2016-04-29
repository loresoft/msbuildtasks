

using System;
using System.IO;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnCheckoutTest
    /// </summary>
    [TestFixture]
    public class SvnCheckoutTest
    {
        private string testDirectory;

        [OneTimeSetUp]
        public void FixtureInit()
        {
            MockBuild buildEngine = new MockBuild();

            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
        }


        [Test(Description="Checkout local repository")]
        public void SvnCheckoutLocal()
        {
            string repoPath = "d:/svn/repo/Test";
            DirectoryInfo dirInfo = new DirectoryInfo(repoPath);
            if (!dirInfo.Exists)
            {
                Assert.Ignore("Repository path '{0}' does not exist", repoPath);
            }

            SvnCheckout checkout = new SvnCheckout();
            checkout.BuildEngine = new MockBuild();

            Assert.IsNotNull(checkout);

            checkout.LocalPath = Path.Combine(testDirectory, @"TestCheckout");
            checkout.RepositoryPath = "file:///" + repoPath + "/trunk";
            bool result = checkout.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(checkout.Revision > 0);
        }

        [Test(Description="Checkout remote repository")]
        public void SvnCheckoutRemote()
        {
            SvnCheckout checkout = new SvnCheckout();
            checkout.BuildEngine = new MockBuild();

            Assert.IsNotNull(checkout);

            checkout.LocalPath = Path.Combine(testDirectory, @"MSBuildTasksCheckout");
            checkout.RepositoryPath =
                "http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk/Source/MSBuild.Community.Tasks.Tests/Subversion";
            checkout.Username = "guest";
            checkout.Password = "guest";
            bool result = checkout.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(checkout.Revision > 0);
        }

        [Test]
        public void SvnCheckoutRemoteCommandLine()
        {
            SvnCheckout checkout = new SvnCheckout();
            string localPath = Path.Combine(testDirectory, @"MSBuildTasksCheckout");
            string remotePath =
                "http://msbuildtasks.tigris.org/svn/msbuildtasks/trunk/Source/MSBuild.Community.Tasks.Tests/Subversion";

            checkout.LocalPath = localPath;
            checkout.RepositoryPath = remotePath;
            checkout.Username = "guest";
            checkout.Password = "guest1";

            string expectedCommand =
                String.Format(
                    "checkout \"{0}\" \"{1}\" --username guest --password guest1 --non-interactive --no-auth-cache",
                    remotePath, localPath);
            string actualCommand = TaskUtility.GetToolTaskCommand(checkout);
            Assert.AreEqual(expectedCommand, actualCommand);
        }
    }
}