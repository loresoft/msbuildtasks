

using System.IO;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnVersionTest
    /// </summary>
    [TestFixture]
    public class SvnVersionTest
    {
        [Test(Description="Test SVN Version of project source directory")]
        public void SvnVersionExecute()
        {
            SvnVersion task = new SvnVersion();
            task.BuildEngine = new MockBuild();

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.LocalPath = Path.Combine(prjRootPath, @"Source");

            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(task.Revision > 0, "Invalid Revision Number");
        }
    }
}