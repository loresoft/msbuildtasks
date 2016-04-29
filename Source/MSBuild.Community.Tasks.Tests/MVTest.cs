

using System;
using System.Text;
using NUnit.Framework;
using System.IO;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for MVTest
    /// </summary>
    [TestFixture]
    public class MVTest
    {
        public MVTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [OneTimeSetUp]
        public void FixtureInit()
        {
            MockBuild buildEngine = new MockBuild();
            TaskUtility.makeTestDirectory(buildEngine);
        }

        [Test]
        public void MVExecute()
        {
            string attribFile = Path.Combine(TaskUtility.TestDirectory, @"moveme.txt");
            File.WriteAllText(attribFile, "This is a test file");

            MV task = new MV();
            task.BuildEngine = new MockBuild();
            task.SourceFiles = TaskUtility.StringArrayToItemArray(attribFile);
            task.DestinationFolder = new TaskItem(Path.Combine(TaskUtility.TestDirectory, ".."));
            task.Execute();

            File.WriteAllText(attribFile, "This is a test file");

            task = new MV();
            task.BuildEngine = new MockBuild();
            task.SourceFiles = TaskUtility.StringArrayToItemArray(attribFile);
            task.DestinationFiles = TaskUtility.StringArrayToItemArray("newme.txt");
            task.Execute();

        }
    }
}
