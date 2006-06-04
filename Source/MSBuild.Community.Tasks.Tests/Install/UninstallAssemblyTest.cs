// $Id$

using System;
using MSBuild.Community.Tasks.Install;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Install
{
    internal class UninstallAssemblyTestWrapper : UninstallAssembly
    {
        public string CommandArguments { get { return this.GenerateCommandLineCommands(); } }
    }

    [TestFixture]
    public class UninstallAssemblyTest
    {
        [Test]
        public void Uninstall_VerifyArguments()
        {
            UninstallAssemblyTestWrapper uninstallTask = new UninstallAssemblyTestWrapper();
            uninstallTask.AssemblyFiles = new TaskItem[] { new TaskItem("testfile.dll") };
            string expectedArgument = "/uninstall";
            Assert.IsTrue(uninstallTask.CommandArguments.Contains(expectedArgument), "Should have include uninstall argument. Actual arguments: " + uninstallTask.CommandArguments);
        }
    }
}
