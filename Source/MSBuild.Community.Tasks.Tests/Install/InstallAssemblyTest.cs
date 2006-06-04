// $Id$

using System;
using MSBuild.Community.Tasks.Install;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Install
{
    internal class InstallAssemblyTestWrapper : InstallAssembly
    {
        public string CommandArguments { get { return this.GenerateCommandLineCommands(); } }
        public string CommandPath { get { return this.GenerateFullPathToTool(); } }
    }

    [TestFixture]
    public class InstallAssemblyTest
    {
        [Test]
        public void Install_SingleAssemblyFile()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            installTask.AssemblyFiles = new TaskItem[] { new TaskItem("testfile.dll") };
            string expectedCommand = "testfile.dll";
            Assert.AreEqual(expectedCommand, installTask.CommandArguments);
        }

        [Test]
        public void Install_MultipleAssemblyFiles()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            installTask.AssemblyFiles = new TaskItem[] 
                {   new TaskItem("C.dll"),
                    new TaskItem("A.dll"),
                    new TaskItem("B.dll") 
                };
            string expectedCommand = "C.dll A.dll B.dll";
            Assert.AreEqual(expectedCommand, installTask.CommandArguments);
        }

        public void Install_MultipleAssemblyFilesAndAssemblyNames()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();

            installTask.AssemblyNames = new TaskItem[] 
                {   new TaskItem("Z,Version=1.0.0.0"),
                    new TaskItem("X,Version=1.0.0.0"),
                    new TaskItem("Y") 
                };
            installTask.AssemblyFiles = new TaskItem[] 
                {   new TaskItem("C.dll"),
                    new TaskItem("A.dll"),
                    new TaskItem("B.dll") 
                };

            string expectedCommand = "C.dll A.dll B.dll /AssemblyName \"Z,Version=1.0.0.0\" /AssemblyName \"X,Version=1.0.0.0\" /AssemblyName Y";
            Assert.AreEqual(expectedCommand, installTask.CommandArguments);
        }

        [Test]
        public void LogFile_NoneSpecified_NoArgument()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            Assert.IsFalse(installTask.CommandArguments.Contains("/LogFile"), "Should not have included LogFile argument. Actual arguments: " + installTask.CommandArguments);
        }

        [Test]
        public void LogFile_UserSpecified_UserFile()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            installTask.LogFile = "mylog.txt";
            string expectedArgument = "/LogFile=mylog.txt";
            Assert.IsTrue(installTask.CommandArguments.Contains(expectedArgument), "Should have specified LogFile argument with user's file. Actual arguments: " + installTask.CommandArguments);
        }

        [Test]
        public void LogFile_BlankSpecified_DisableLogging()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            installTask.LogFile = " ";
            string expectedArgument = "/LogFile=";
            Assert.IsTrue(installTask.CommandArguments.Contains(expectedArgument), "Should have specified LogFile argument with no value. Actual arguments: " + installTask.CommandArguments);
        }

        [Test]
        public void ShowCallStack_False_NoArgument()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            Assert.IsFalse(installTask.CommandArguments.Contains("/ShowCallStack"), "Should not have included ShowCallStack argument. Actual arguments: " + installTask.CommandArguments);
        }

        [Test]
        public void ShowCallStack_True_IncludeArgument()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            installTask.ShowCallStack = true;
            Assert.IsTrue(installTask.CommandArguments.Contains("/ShowCallStack"), "Should have included ShowCallStack argument. Actual arguments: " + installTask.CommandArguments);
        }

        [Test]
        public void ToolPath_NoneSpecified_FrameworkPath()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            string expectedPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(typeof(string).Assembly.Location),
                "InstallUtil.exe"
                );
            Assert.AreEqual(expectedPath, installTask.CommandPath, "Should have used .NET Framework path.");
        }

        [Test]
        public void ToolPath_UserSpecified_UserPath()
        {
            InstallAssemblyTestWrapper installTask = new InstallAssemblyTestWrapper();
            installTask.ToolPath = @"c:\apps";
            string expectedPath = @"c:\apps\InstallUtil.exe";
            Assert.AreEqual(expectedPath, installTask.CommandPath, "Should have used user defined path.");
        }


    }
}
