// $Id$

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class WebDownloadTest
    {
        private string testDirectory;

        [SetUp()]
        public void Setup()
        {
            MockBuild buildEngine = new MockBuild();

            testDirectory = TaskUtility.makeTestDirectory(buildEngine);

        }

        [Test(Description = "Download www.microsoft.com/default.aspx")]
        public void WebDownloadExecute()
        {
            WebDownload task = new WebDownload();
            task.BuildEngine = new MockBuild();
            task.FileUri = "http://www.microsoft.com/default.aspx";
            string downloadFile = Path.Combine(testDirectory, "microsoft.html");
            task.FileName = downloadFile;
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists(downloadFile), "No download file");

        }
    }
}
