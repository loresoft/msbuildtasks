// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for FtpTest
    /// </summary>
    [TestClass]
    public class FtpUploadTest
    {
        public FtpUploadTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FtpExecute()
        {
            FtpUpload task = new FtpUpload();
            task.BuildEngine = new MockBuild();
            task.LocalFile = @"D:\svn\repo.zip";
            task.RemoteUri = "ftp://localhost/repo.zip";
            task.Execute();

        }
    }
}
