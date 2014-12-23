

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
            task.AssemblyInformationalVersion = "1.2.3.4";
            task.GenerateClass = true;
            
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
            task.AssemblyInformationalVersion = "1.2.3.4";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists(outputFile), "File missing: " + outputFile);

        }

        [Test(Description = "Create VersionInfo in CPP/CLI")]
        [Explicit]
        public void AssemblyInfoCPP()
        {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cpp";
            string outputFile = Path.Combine(testDirectory, "VersionInfo.cpp");
            task.OutputFile = outputFile;
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            task.AssemblyInformationalVersion = "1.2.3.4";
            task.SkipVerification = true;
            task.UnmanagedCode = true;   
            task.GenerateClass = true;

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
            task.AssemblyInformationalVersion = "1.2.3.4";
            task.AssemblyKeyFile = @"..\MSBuild.Community.Tasks\MSBuild.Community.Tasks.snk";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists(outputFile), "File missing: " + outputFile);

        }

        [Test]
        public void AssemblyInfoFileShouldHaveUtf8ByteOrderMark()
        {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";
            string outputFile = Path.Combine(testDirectory, "AssemblyInfoBOM.cs");
            task.OutputFile = outputFile;
            task.AssemblyTitle = "AssemblyInfoTask";
            task.AssemblyCopyright = "Copyright � Ignaz Kohlbecker 2006";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            byte[] firstBytesOfFile = new byte[3];
            File.OpenRead(outputFile).Read(firstBytesOfFile, 0, 3);

            byte[] utf8Bom = UTF8Encoding.UTF8.GetPreamble();
            
            Assert.AreEqual(utf8Bom, firstBytesOfFile, "The expected UTF8 BOM marker was not found on the generated file.");
            
        }

        [Test]
        public void IncludeNeutralResourceLanguage()
        {
            string outputFile = Path.Combine(testDirectory, "AssemblyInfoNeutralResource.cs");
            AssemblyInfo task = CreateCSAssemblyInfo(outputFile); 
            task.NeutralResourcesLanguage = "en-US";

            Assert.IsTrue(task.Execute(), "Execute failed");
            Assert.IsTrue(File.Exists(outputFile));

            string content;
            using(StreamReader stream = File.OpenText(outputFile))
            {
                content = stream.ReadToEnd();
            }
            Assert.IsNotNull(content);
            Assert.That(content.Contains("assembly: System.Resources.NeutralResourcesLanguage(\"en-US\")"));
        }
    
        [Test(Description="Creates an assembly info which has InternalsVisibleTo attribute")]
        public void IncludeInternalsVisibleTo()
       { 
            string outputFile = Path.Combine(testDirectory, "InternalsVisibleTo.cs");
            AssemblyInfo task = CreateCSAssemblyInfo(outputFile);
            task.InternalsVisibleTo = "UnitTests";

            Assert.IsTrue(task.Execute(), "Execute failed");
            Assert.IsTrue(File.Exists(outputFile));

            string content;
            using (StreamReader stream = File.OpenText(outputFile))
            {
                content = stream.ReadToEnd();
            }
            Assert.IsNotNull(content);
            Assert.That(content.Contains("assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"UnitTests\")"));
        }

        [Test(Description = "Creates an assembly info which has AllowPartiallyTrustedCallers attribute")]
        public void IncludeAllowPartiallyTrustedCallers()
        {
            string outputFile = Path.Combine(testDirectory, "AllowPartiallyTrustedCallers.cs");
            AssemblyInfo task = CreateCSAssemblyInfo(outputFile);
            task.AllowPartiallyTrustedCallers = true;

            Assert.IsTrue(task.Execute(), "Execute failed");
            Assert.IsTrue(File.Exists(outputFile));

            string content;
            using (StreamReader stream = File.OpenText(outputFile))
            {
                content = stream.ReadToEnd();
            }
            Assert.IsNotNull(content);
            Assert.That(content.Contains("assembly: System.Security.AllowPartiallyTrustedCallers()"));
        }

        private AssemblyInfo CreateCSAssemblyInfo(string outputFile)
        {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";

            task.OutputFile = outputFile;
            task.AssemblyTitle = "AssemblyInfoTask";
            task.AssemblyDescription = "AssemblyInfo Description";
            task.AssemblyConfiguration = "";
            task.AssemblyCompany = "Company Name, LLC";
            task.AssemblyProduct = "AssemblyInfoTask";
            task.AssemblyCopyright = "Copyright (c) Company Name, LLC 2005";
            return task;
        }
    }
}
