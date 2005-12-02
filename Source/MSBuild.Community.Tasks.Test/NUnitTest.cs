// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for NUnitTest
    /// </summary>
    [TestClass]
    public class NUnitTest
    {
        public NUnitTest()
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
        public void NUnitExecute()
        {
            string[] assemblies = new string[] { @"C:\Program Files\NUnit 2.2.3\bin\nunit.framework.tests.dll" };
            TaskItem[] items = TaskUtility.StringArrayToItemArray(assemblies);

            NUnit task = new NUnit();
            task.BuildEngine = new MockBuild();            
            task.Assemblies = items;
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
