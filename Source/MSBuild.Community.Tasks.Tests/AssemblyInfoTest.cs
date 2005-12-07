// $Id$

using System;
using System.Text;
using System.Collections.Generic;
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
        public AssemblyInfoTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void AssemblyInfoExecute()
        {
            AssemblyInfo task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "cs";
            task.OutputFile = "VersionInfo.cs";
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists("VersionInfo.cs"));

            task = new AssemblyInfo();
            task.BuildEngine = new MockBuild();
            task.CodeLanguage = "vb";
            task.OutputFile = "VersionInfo.vb";
            task.AssemblyVersion = "1.2.3.4";
            task.AssemblyFileVersion = "1.2.3.4";
            Assert.IsTrue(task.Execute(), "Execute Failed");

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
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(File.Exists("AssemblyInfo.cs"));

        }
    }
}
