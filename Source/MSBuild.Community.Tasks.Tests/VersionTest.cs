// $Id$

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for VersionTest
    /// </summary>
    [TestFixture]
    public class VersionTest
    {
        private string testDirectory;
        private Version task;
        private string versionFile;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            testDirectory = TaskUtility.makeTestDirectory(new MockBuild());
            versionFile = Path.Combine(testDirectory, @"version.txt");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (File.Exists(versionFile)) { File.Delete(versionFile); }
        }

        [SetUp]
        public void TestSetup()
        {
            if (File.Exists(versionFile)) { File.Delete(versionFile); }
            task = new Version();
            task.BuildEngine = new MockBuild();
        }

        [Test]
        public void SpecifyFile_FileDoesNotExist_CreateWith1_0_0_0()
        {
            task.VersionFile = versionFile;
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual(1, task.Major);
            Assert.AreEqual(0, task.Minor);
            Assert.AreEqual(0, task.Build);
            Assert.AreEqual(0, task.Revision);

            string fileContents = File.ReadAllText(versionFile);
            Assert.AreEqual("1.0.0.0", fileContents);
        }

        [Test]
        public void SpecifyFile_FileExists_IncrementAndOverwrite()
        {
            File.AppendAllText(versionFile, "1.2.3.4");
            task.VersionFile = versionFile;
            task.BuildType = "Increment";
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual(1, task.Major);
            Assert.AreEqual(2, task.Minor);
            Assert.AreEqual(4, task.Build);
            Assert.AreEqual(5, task.Revision);

            string fileContents = File.ReadAllText(versionFile);
            Assert.AreEqual("1.2.4.5", fileContents);
        }

        [Test]
        public void BuildType_Increment()
        {
            task.Build = 7;
            task.BuildType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual(8, task.Build);
        }

        [Test]
        public void BuildType_Automatic_DaysSinceMillenium()
        {
            task.BuildType = "Automatic";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            int expected = (int)DateTime.Today.Subtract(new DateTime(2000, 1, 1)).Days;
            Assert.AreEqual(expected, task.Build);
        }

        [Test]
        public void BuildType_Date_ReturnSixDigitDate()
        {
            task.BuildType = "Date";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            DateTime dDate = DateTime.Now;
            int _month = dDate.Month * 100;
            int _day = dDate.Day;
            int _year = (dDate.Year % 2000) * 10000;
            int buildDate = _year + _month + _day;

            Assert.AreEqual(buildDate, task.Build);
        }

        [Test]
        public void BuildType_DateIncrement()
        {
            DateTime startDate = new DateTime(2002, 12, 5);
            int daysSinceStartDate = DateTime.Today.Subtract(startDate).Days;

            task.StartDate = startDate.ToString();
            task.BuildType = "DateIncrement";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(task.Build, daysSinceStartDate);
        }

        [Test]
        public void BuildType_None_NoChange()
        {
            task.BuildType = "None";
            task.Build = 42;

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(42, task.Build);
        }

        [Test]
        public void RevisionType_Automatic_SecondsSinceMidnight()
        {
            task.RevisionType = "Automatic";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            int expected = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            // task should execute within a second
            Assert.Greater(task.Revision, expected - 2);
            Assert.Less(task.Revision, expected + 2);
        }


        [Test]
        public void RevisionType_Increment()
        {
            task.Revision = 4;
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(5, task.Revision);
        }

        [Test]
        public void RevisionType_Increment_WithBuildDateIncrement_SameDay_IncrementsRevision()
        {
            task.StartDate = DateTime.Today.ToString();
            task.Build = 0;
            task.Revision = 4;
            task.BuildType = "DateIncrement";
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(5, task.Revision);
        }

        [Test]
        public void RevisionType_Increment_WithBuildDateIncrement_DifferentDay_ResetsRevision()
        {
            DateTime startDate = new DateTime(2006, 12, 1);
            int daysSinceStartDate = DateTime.Today.Subtract(startDate).Days;

            task.StartDate = startDate.ToString();
            task.Build = daysSinceStartDate + 1;
            task.Revision = 4;
            task.BuildType = "DateIncrement";
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(0, task.Revision);
        }

        [Test]
        public void RevisionType_None_NoChange()
        {
            task.RevisionType = "None";
            task.Revision = 24;

            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(24, task.Revision);
        }

        [Test]
        public void VerifyDefaults()
        {
            Assert.AreEqual(1, task.Major);
            Assert.AreEqual(0, task.Minor);
            Assert.AreEqual(0, task.Build);
            Assert.AreEqual(0, task.Revision);
            Assert.AreEqual("None", task.BuildType);
            Assert.AreEqual("None", task.RevisionType);
        }

        [Test]
        public void Unrecognized_BuildType()
        {
            task.BuildType = "Invalid";
            Assert.IsFalse(task.Execute(), "Task should have failed.");
        }

        [Test]
        public void Unrecognized_RevisionType()
        {
            task.RevisionType = "Invalid";
            Assert.IsFalse(task.Execute(), "Task should have failed.");
        }
    }
}
