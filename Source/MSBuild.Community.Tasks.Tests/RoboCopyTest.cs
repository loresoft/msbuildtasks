using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class RoboCopyTest
    {
        [SetUp()]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown()]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test()]
        public void Copy()
        {
            MockBuild buildEngine = new MockBuild();
            string testDirectory = TaskUtility.makeTestDirectory(buildEngine);
            
            RoboCopy copy = new RoboCopy();
            copy.BuildEngine = buildEngine;
            copy.SourceFolder = @"..\..\..\";
            copy.DestinationFolder = Path.Combine(testDirectory, "RoboCopy");
            copy.Mirror = true;
            copy.ExcludeFolders = new string[] { ".svn", "bin", "obj", "Test" };
            copy.ExcludeFiles = new string[] { "*.resx", "*.csproj", "*.webinfo", "*.log" };
            bool result = copy.Execute();

            Assert.IsTrue(result);
        }
    }
}
