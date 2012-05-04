

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for UnzipTest
    /// </summary>
    [TestFixture]
    public class UnzipTest
    {
        [Test(Description="Unzip a zip archive")]
        public void UnzipExecute()
        {
            string testDir = TaskUtility.TestDirectory;
            string zipFileName = Path.Combine(testDir, ZipTest.ZIP_FILE_NAME);

            if (!File.Exists(zipFileName))
            {
                Assert.Ignore("Zip file \"{0}\" not found; first run test that creates it", zipFileName);
            }

            ZipTest zip = new ZipTest();
            zip.ZipExecute();

            Unzip task = new Unzip();
            task.BuildEngine = new MockBuild();
            task.ZipFileName = zipFileName;
            task.TargetDirectory = Path.Combine(testDir, @"Backup");

            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
