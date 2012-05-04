
using System;
using NUnit.Framework;
using MSBuild.Community.Tasks.AspNet;

namespace MSBuild.Community.Tasks.Tests.AspNet
{
    [TestFixture]
    public class InstallAspNetTest
    {
        InstallAspNet aspnetTask;

        [SetUp]
        public void CreateTask()
        {
            aspnetTask = new InstallAspNet();
            aspnetTask.BuildEngine = new MockBuild();
        }

        [Test]
        public void CombineValidProperties()
        {
            aspnetTask.Path = "test";
            aspnetTask.Recursive = false;
            Assert.IsTrue(aspnetTask.IsValidPropertyCombinations(), "You should be able to set a path and Recursive=false.");
        }

        [Test]
        public void ClientScriptsOnly()
        {
            aspnetTask.ClientScriptsOnly = true;
            string expectedCommand = "-c";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void ClientScriptsOnly_RecursiveFalse()
        {
            aspnetTask.ClientScriptsOnly = true;
            aspnetTask.Recursive = false;
            Assert.IsFalse(aspnetTask.IsValidPropertyCombinations(), "You should not be able to set Recursive=false and ClientScriptsOnly=true");
        }

        [Test]
        public void ClientScriptsOnly_PathSpecified()
        {
            aspnetTask.ClientScriptsOnly = true;
            aspnetTask.Path = "Test";
            Assert.IsFalse(aspnetTask.IsValidPropertyCombinations(), "You should not be able to set a path and ClientScriptsOnly=true");
        }

        [Test]
        public void ClientScriptsOnly_ApplyScriptMapsSpecified()
        {
            aspnetTask.ClientScriptsOnly = true;
            aspnetTask.ApplyScriptMaps = "Always";
            Assert.IsFalse(aspnetTask.IsValidPropertyCombinations(), "You should not be able to apply script maps when ClientScriptsOnly=true");
        }

        [Test]
        public void InstallAspNetOnServerUsingDefaultBehavior()
        {
            string expectedCommand = "-i";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void InstallAspNetWithoutScriptMaps()
        {
            aspnetTask.ApplyScriptMaps = "Never";
            string expectedCommand = "-ir";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void InstallAspNetWithScriptMapsUnlessSomeExist()
        {
            aspnetTask.ApplyScriptMaps = "IfNoneExist";
            string expectedCommand = "-iru";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void InstallAspNetWithScriptMapsOverridingNewerVersions()
        {
            aspnetTask.ApplyScriptMaps = "Always";
            string expectedCommand = "-r";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void UnrecognizedValueForApplyScriptMaps()
        {
            aspnetTask.ApplyScriptMaps = "something";
            Assert.IsFalse(aspnetTask.IsValidPropertyCombinations(), "You must use one of the pre-defined values for ApplyScriptMaps.");
        }

        [Test]
        public void RegisterScriptMapsForAFolder()
        {
            aspnetTask.Path = "W3SVC/2/Root/test";
            string expectedCommand = "-s W3SVC/2/Root/test";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void RegisterScriptMapsForAFolderNonRecursively()
        {
            aspnetTask.Path = "W3SVC/2/Root/test";
            aspnetTask.Recursive = false;
            string expectedCommand = "-sn W3SVC/2/Root/test";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void RegisterScriptMapsForAFolderAssumeDefaultWebSite()
        {
            aspnetTask.Path = "test";
            string expectedCommand = "-s W3SVC/1/Root/test";
            Assert.AreEqual(expectedCommand, TaskUtility.GetToolTaskCommand(aspnetTask));
        }

        [Test]
        public void InstallAspNet11()
        {
            if (!System.IO.Directory.Exists(Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Microsoft.NET\Framework\v1.1.4322")))
            {
                Assert.Ignore(".NET Framework 1.1 is not installed.");
            }
            aspnetTask.Version = "Version11";
            Assert.IsTrue(TaskUtility.GetToolTaskToolPath(aspnetTask).Contains("v1.1.4322"), "Should have used executable in v1.1 folder.");
        }

        [Test]
        public void InstallAspNet20()
        {
            aspnetTask.Version = "Version20";
            Assert.IsTrue(TaskUtility.GetToolTaskToolPath(aspnetTask).Contains("v2.0.50727"), "Should have used executable in v2.0 folder.");
        }


    }
}
