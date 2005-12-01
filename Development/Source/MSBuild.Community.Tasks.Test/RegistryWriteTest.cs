// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for RegistryWriteTest
    /// </summary>
    [TestClass]
    public class RegistryWriteTest
    {
        public RegistryWriteTest()
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
        public void RegistryWriteExecute()
        {
            RegistryWrite task = new RegistryWrite();
            task.BuildEngine = new MockBuild();
            task.KeyName = @"HKEY_CURRENT_USER\SOFTWARE\MSBuildTasks";
            task.ValueName = "RegistryWrite";
            task.Value = "Test Write";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            
        }
    }
}
