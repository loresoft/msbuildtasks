// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSBuild.Community.Tasks.Math;

namespace MSBuild.Community.Tasks.Test.Math
{
    /// <summary>
    /// Summary description for MultipleTest
    /// </summary>
    [TestClass]
    public class MultipleTest
    {
        public MultipleTest()
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
        public void MultipleExecute()
        {
            Multiple task = new Multiple();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "3", "4" };
            task.Execute();

            Assert.AreEqual("12", task.Result);

            task = new Multiple();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "1.1", "2.1" };
            task.Execute();

            Assert.AreEqual("2.31", task.Result);

            task = new Multiple();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "5", "6", "4" };
            task.Execute();

            Assert.AreEqual("120", task.Result);
        }
    }
}
