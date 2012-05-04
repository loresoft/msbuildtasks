

using System;
using MSBuild.Community.Tasks.Install;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Install
{
    [TestFixture]
    public class InstallAssemblyTest
    {
        private InstallAssembly installTask;
        
        [SetUp]
        public void Setup()
        {
            installTask = new InstallAssembly();
        }

        [Test]
        public void Install_SingleAssemblyFile()
        {
            installTask.AssemblyFiles = new TaskItem[] { new TaskItem("testfile.dll") };
            string expectedCommand = "testfile.dll";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(installTask));
        }

        [Test]
        public void Install_MultipleAssemblyFiles()
        {
            installTask.AssemblyFiles = new TaskItem[] 
                {   new TaskItem("C.dll"),
                    new TaskItem("A.dll"),
                    new TaskItem("B.dll") 
                };
            string expectedCommand = "C.dll A.dll B.dll";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(installTask));
        }

        public void Install_MultipleAssemblyFilesAndAssemblyNames()
        {

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
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(installTask));
        }

        [Test]
        public void LogFile_NoneSpecified_NoArgument()
        {
            string command = TaskUtility.GetToolTaskCommand(installTask);
            Assert.IsFalse(command.Contains("/LogFile"), "Should not have included LogFile argument. Actual arguments: " + command);
        }

        [Test]
        public void LogFile_UserSpecified_UserFile()
        {
            installTask.LogFile = "mylog.txt";
            string expectedArgument = "/LogFile=mylog.txt";
            string command = TaskUtility.GetToolTaskCommand(installTask);
            Assert.IsTrue(command.Contains(expectedArgument), "Should have specified LogFile argument with user's file. Actual arguments: " + command);
        }

        [Test]
        public void LogFile_BlankSpecified_DisableLogging()
        {
            installTask.LogFile = " ";
            string expectedArgument = "/LogFile=";
            string command = TaskUtility.GetToolTaskCommand(installTask);
            Assert.IsTrue(command.Contains(expectedArgument), "Should have specified LogFile argument with no value. Actual arguments: " + command);
        }

        [Test]
        public void ShowCallStack_False_NoArgument()
        {
            string command = TaskUtility.GetToolTaskCommand(installTask);
            Assert.IsFalse(command.Contains("/ShowCallStack"), "Should not have included ShowCallStack argument. Actual arguments: " + command);
        }

        [Test]
        public void ShowCallStack_True_IncludeArgument()
        {
            installTask.ShowCallStack = true;
            string command = TaskUtility.GetToolTaskCommand(installTask);
            Assert.IsTrue(command.Contains("/ShowCallStack"), "Should have included ShowCallStack argument. Actual arguments: " + command);
        }

        [Test]
        public void ToolPath_NoneSpecified_FrameworkPath()
        {
            string expectedPath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(typeof(string).Assembly.Location),
                "InstallUtil.exe"
                );
            Assert.AreEqual(expectedPath, TaskUtility.GetToolTaskToolPath(installTask), "Should have used .NET Framework path.");
        }

        [Test]
        public void ToolPath_UserSpecified_UserPath()
        {
            installTask.ToolPath = @"c:\apps";
            string expectedPath = @"c:\apps\InstallUtil.exe";
            Assert.AreEqual(expectedPath, TaskUtility.GetToolTaskToolPath(installTask), "Should have used user defined path.");
        }


    }
}
