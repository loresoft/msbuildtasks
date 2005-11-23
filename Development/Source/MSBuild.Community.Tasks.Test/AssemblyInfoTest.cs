// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSBuild.Community.Tasks.Test
{
    /// <summary>
    /// Summary description for AssemblyInfoTest
    /// </summary>
    [TestClass]
    public class AssemblyInfoTest
    {
        public AssemblyInfoTest()
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
        public void AssemblyInfoExecute()
        {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";
            task.OutputFile = "VersionInfo.cs";
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            task.Execute();

            Assert.IsTrue(File.Exists("VersionInfo.cs"));

            task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "vb";
            task.OutputFile = "VersionInfo.vb";
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            task.Execute();

            Assert.IsTrue(File.Exists("VersionInfo.vb"));

            task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";
            task.OutputFile = "AssemblyInfo.cs";
            task.AssemblyTitle = "AssemblyInfoTask";
            task.AssemblyDescription = "AssemblyInfo Description";
            task.AssemblyConfiguration = "";
            task.AssemblyCompany = "Company Name, LLC";
            task.AssemblyProduct = "AssemblyInfoTask";
            task.AssemblyCopyright = "Copyright (c) Company Name, LLC 2005";
            task.AssemblyTrademark = "";
            task.ComVisible = false;
            task.CLSCompliant = true;
            task.Guid = "d038566a-1937-478a-b5c5-b79c4afb253d";
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            task.Execute();

            Assert.IsTrue(File.Exists("AssemblyInfo.cs"));

        }
    }
}
