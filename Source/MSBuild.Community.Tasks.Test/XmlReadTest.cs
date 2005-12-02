// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for XmlReadTest
    /// </summary>
    [TestClass]
    public class XmlReadTest
    {
        public XmlReadTest()
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
        public void XmlReadExecute()
        {
            XmlRead task = new XmlRead();
            task.BuildEngine = new MockBuild();
            task.XmlFileName = @"..\..\..\MSBuild.Community.Tasks\Subversion.proj";
            task.XPath = "string(/n:Project/n:PropertyGroup/n:MSBuildCommunityTasksPath/text())";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.Prefix = "n";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            task.XPath = "/n:Project/n:Target/@Name";
            Assert.IsTrue(task.Execute(), "Execute Failed");



        }
    }
}
