// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for FtpTest
    /// </summary>
    [TestFixture]
    public class FtpUploadTest
    {
        public FtpUploadTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void FtpExecute()
        {
            ZipTest zip = new ZipTest();
            zip.ZipExecute(); 
            
            FtpUpload task = new FtpUpload();
            task.BuildEngine = new MockBuild();
            task.LocalFile = @"MSBuild.Community.Tasks.zip";
            task.RemoteUri = "ftp://localhost/MSBuild.Community.Tasks.zip";
            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
