// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSBuild.Community.Tasks.Subversion;

namespace MSBuild.Community.Tasks.Test.Subversion
{
    /// <summary>
    /// Summary description for SvnExportTest
    /// </summary>
    [TestClass]
    public class SvnExportTest
    {
        public SvnExportTest()
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
        public void SvnExporeExecute()
        {
            SvnExport export = new SvnExport();
            export.BuildEngine = new MockBuild();

            Assert.IsNotNull(export);

            export.LocalPath = @"Test";
            export.RepositoryPath = "file:///d:/svn/repo/Test/trunk";
            bool result = export.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(export.Revision > 0);
        }
    }
}
