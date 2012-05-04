

using System;
using MSBuild.Community.Tasks.Install;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Install
{

    [TestFixture]
    public class UninstallAssemblyTest
    {
        [Test]
        public void Uninstall_VerifyArguments()
        {
            UninstallAssembly uninstallTask = new UninstallAssembly();
            uninstallTask.AssemblyFiles = new TaskItem[] { new TaskItem("testfile.dll") };
            string expectedArgument = "/uninstall";
            string command = TaskUtility.GetToolTaskCommand(uninstallTask);
            Assert.IsTrue(command.Contains(expectedArgument), "Should have include uninstall argument. Actual arguments: " + command);
        }
    }
}
