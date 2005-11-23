// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for ZipTest
    /// </summary>
    [TestClass]
    public class ZipTest
    {
        public ZipTest()
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
        public void ZipExecute()
        {
            Zip task = new Zip();
            task.BuildEngine = new MockBuild();

            string[] files = Directory.GetFiles(@"D:\svn\repo", "*.*", SearchOption.AllDirectories);
            Array.Reverse(files, 0, files.Length);

            TaskItem[] items = TaskUtility.StringArrayToItemArray(files);

            task.Files = items;
            task.ZipFile = @"D:\svn\repo.zip";

            bool result = task.Execute();

            Assert.IsTrue(result, "Execute Failed");
            Assert.IsTrue(File.Exists(@"D:\svn\repo.zip"), "Zip file not found");
        }
    }
}
