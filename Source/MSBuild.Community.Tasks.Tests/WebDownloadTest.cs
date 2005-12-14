using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class WebDownloadTest
    {
        [SetUp()]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown()]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test()]
        public void WebDownloadExecute()
        {
            WebDownload task = new WebDownload();
            task.BuildEngine = new MockBuild();
            task.FileUri = "http://www.microsoft.com/default.aspx";
            task.FileName = "microsoft.html";
            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
