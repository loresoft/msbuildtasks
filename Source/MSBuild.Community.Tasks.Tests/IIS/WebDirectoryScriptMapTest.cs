
using System;
using NUnit.Framework;
using MSBuild.Community.Tasks.IIS;

namespace MSBuild.Community.Tasks.Tests.IIS
{
    [TestFixture]
    public class WebDirectoryScriptMapTest
    {
        const string TestWebDirectoryName = "WebDirectoryScriptMapTests";
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
        public void MapToAspNet()
        {
            WebDirectoryScriptMap task = new WebDirectoryScriptMap();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.Extension = "msbct";
            task.MapToAspNet = true;
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }

        [Test]
        public void AddScriptMap_GetOnly()
        {
            WebDirectoryScriptMap task = new WebDirectoryScriptMap();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.Extension = "msbct";
            task.Verbs = "GET";
            task.MapToAspNet = true;
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }

        [Test]
        public void AddScriptMap_VerifyFileExists()
        {
            WebDirectoryScriptMap task = new WebDirectoryScriptMap();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.Extension = "msbct";
            task.VerifyFileExists = true;
            task.MapToAspNet = true;
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }

        [Test]
        public void AddScriptMap_using_wildcard()
        {
            WebDirectoryScriptMap task = new WebDirectoryScriptMap();
            task.BuildEngine = new MockBuild();
            task.VirtualDirectoryName = TestWebDirectoryName;
            task.Extension = "*";
            task.VerifyFileExists = false;
            task.MapToAspNet = true;
            Assert.IsTrue(task.Execute(), "Task should have succeeded.");
        }


    }
}
