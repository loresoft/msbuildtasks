
using System;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.IIS;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.IIS
{
    [TestFixture]
    public class WebDirectorySettingTest
    {
        const string TestWebDirectoryName = "WebDirectorySettingTests";
        string TestWebDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        #region Environment Setup

        [OneTimeSetUp]
        public void CreateTestWebDirectory()
        {
            if (!TaskUtility.IsMinimumIISVersionInstalled("localhost", 5, 0))
            {
                Assert.Ignore(@"IIS 5.0 was not found on the machine.  IIS 5.0 is required to run this test.");
            }

            WebDirectoryCreate task = new WebDirectoryCreate();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.VirtualDirectoryPhysicalPath = TestWebDirectoryPath;
            task.AuthAnonymous = true;
            if (!task.Execute())
            {
                Assert.Ignore("Unable to create test web directory. IIS is probably not available.");
            }
        }

        [OneTimeTearDown]
        public void RemoveTestWebDirectory()
        {
            WebDirectoryDelete task = new WebDirectoryDelete();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.Execute();
        }

        #endregion

        [Test]
        public void ReadValue_SettingExists_ValueReturned()
        {
            WebDirectorySetting task = new WebDirectorySetting();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.SettingName = "Path";
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
            Assert.AreEqual(TestWebDirectoryPath.ToLower(), task.SettingValue.ToLower(), "The configuration setting was not read.");
        }

        [Test]
        public void ReadValue_SettingDoesNotExist_TaskFails()
        {
            WebDirectorySetting task = new WebDirectorySetting();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.SettingName = "SomethingThatDoesNotExist";
            Assert.IsFalse(task.Execute(), "Task should have failed.");
        }

        [Test]
        public void ReadMultiValuedSetting_FirstValueReturned()
        {
            WebDirectorySetting task = new WebDirectorySetting();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.SettingName = "DefaultDoc";
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
            Assert.IsTrue(task.SettingValue.Length > 0, "A value should have been returned.");
        }


        [Test]
        public void SetValue_SettingExists_NewValueReturned()
        {
            WebDirectorySetting task = new WebDirectorySetting();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.SettingName = "AuthAnonymous";
            task.SettingValue = "False";
            string expectedValue = "False";
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
            Assert.AreEqual(expectedValue, task.SettingValue, "The configuration setting was not set.");
        }

        [Test]
        public void SetValue_SettingDoesNotExist_TaskFails()
        {
            WebDirectorySetting task = new WebDirectorySetting();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.SettingName = "SomethingThatDoesNotExist";
            task.SettingValue = "Whatever";
            Assert.IsFalse(task.Execute(), "Task should have failed.");
        }

        [Test]
        public void ReadValue_DirectoryDoesNotExist_TaskFails()
        {
            WebDirectorySetting task = new WebDirectorySetting();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = "SomethingThatDoesNotExist";
            task.SettingName = "SomethingThatDoesNotExist";
            Assert.IsFalse(task.Execute(), "Task should have failed.");
        }

    }
}
