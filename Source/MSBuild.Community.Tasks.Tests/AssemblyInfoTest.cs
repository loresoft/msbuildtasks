// $Id$

using System;
using System.Text;
using System.IO;
using NUnit.Framework;


namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for AssemblyInfoTest
    /// </summary>
    [TestFixture]    
    public class AssemblyInfoTest
    {
        private string testDirectory;

        [TestFixtureSetUp]
        public void FixtureInit() 
        {
            MockBuild buildEngine = new MockBuild();

            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
        }

        [Test(Description = "Create VersionInfo in C#")]
        public void AssemblyInfoCS() {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";
            string outputFile = Path.Combine(testDirectory, "VersionInfo.cs");
            task.OutputFile = outputFile;
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists(outputFile), "File missing: " + outputFile);

        }

        [Test(Description = "Create VersionInfo in VB")]
        public void AssemblyInfoVB() {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "vb";
            string outputFile = Path.Combine(testDirectory, "VersionInfo.vb");
            task.OutputFile = outputFile;
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists(outputFile), "File missing: " + outputFile);

        }

        [Test(Description = "Create AssemblyInfo in C#")]
        public void AssemblyInfoExecute() {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";
            string outputFile = Path.Combine(testDirectory, "AssemblyInfo.cs");
            task.OutputFile = outputFile;
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
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists(outputFile), "File missing: " + outputFile);

        }
    }
}
