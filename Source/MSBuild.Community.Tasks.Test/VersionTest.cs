// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for VersionTest
    /// </summary>
    [TestClass]
    public class VersionTest
    {
        public VersionTest()
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
        public void VersionExecute()
        {
            Version task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = "number.txt";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual(1, task.Major);
            Assert.AreEqual(0, task.Minor); 
            Assert.AreEqual(0, task.Build);
            Assert.AreEqual(0, task.Revision);

            task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = @"version.txt";
            task.BuildType = "Increment";
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual(1, task.Major);
            Assert.AreEqual(0, task.Minor);


            task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = @"version.txt";
            task.BuildType = "Automatic";
            task.RevisionType = "Automatic";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual(1, task.Major);
            Assert.AreEqual(0, task.Minor); 
            
        }
    }
}
