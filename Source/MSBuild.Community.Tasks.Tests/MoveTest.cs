

using System;
using System.Text;
using NUnit.Framework;
using System.IO;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for MoveTest
    /// </summary>
    [TestFixture]
    public class MoveTest
    {
        public MoveTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestFixtureSetUp]
        public void FixtureInit()
        {
            MockBuild buildEngine = new MockBuild();
            TaskUtility.makeTestDirectory(buildEngine);
        }

        [Test]
        public void MoveExecute()
        {
            string attribFile = Path.Combine(TaskUtility.TestDirectory, @"moveme.txt");
            File.WriteAllText(attribFile, "This is a test file");

            Move task = new Move();
            task.BuildEngine = new MockBuild();
            task.SourceFiles = TaskUtility.StringArrayToItemArray(attribFile);
            task.DestinationFolder = new TaskItem(Path.Combine(TaskUtility.TestDirectory, ".."));
            task.Execute();

            File.WriteAllText(attribFile, "This is a test file");

            task = new Move();
            task.BuildEngine = new MockBuild();
            task.SourceFiles = TaskUtility.StringArrayToItemArray(attribFile);
            task.DestinationFiles = TaskUtility.StringArrayToItemArray("newme.txt");
            task.Execute();

        }
    }
}
