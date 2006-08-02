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

        [TestFixtureSetUp]
        public void FixtureInit()
        {
            MockBuild buildEngine = new MockBuild();

            testDirectory = TaskUtility.makeTestDirectory(buildEngine);
        
        }

        [Test]
		public void VersionExecute()
		{
            string numberFile = Path.Combine(testDirectory, @"number.txt");
            
            Version task = new Version();
			task.BuildEngine = new MockBuild();
			task.VersionFile = numberFile;
			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.AreEqual(1, task.Major);
			Assert.AreEqual(0, task.Minor);
			Assert.AreEqual(0, task.Build);
			Assert.AreEqual(0, task.Revision);

            string versionFile = Path.Combine(testDirectory, @"version.txt");
            
            task = new Version();
			task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
			task.BuildType = "Increment";
			task.RevisionType = "Increment";
			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.AreEqual(1, task.Major);
			Assert.AreEqual(0, task.Minor);


			task = new Version();
			task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            task.BuildType = "Automatic";
			task.RevisionType = "Automatic";
			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.AreEqual(1, task.Major);
			Assert.AreEqual(0, task.Minor);

			task = new Version();
			task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            task.BuildType = "Date";
			task.RevisionType = "Increment";
			Assert.IsTrue(task.Execute(), "Execute Failed");

			Assert.AreEqual(1, task.Major);
			Assert.AreEqual(0, task.Minor);

			DateTime dDate = DateTime.Now;
			int _month = dDate.Month * 100;
			int _day = dDate.Day;
			int _year = (dDate.Year % 2000) * 10000;
			int buildDate = _year + _month + _day;

			Assert.AreEqual(buildDate, task.Build);

            task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;

            Assert.AreEqual("None", task.BuildType);

            task.BuildType = string.Empty; //This should have it default to "None"
            Assert.AreEqual("None", task.BuildType);

            task.BuildType = null;
            Assert.AreEqual("None", task.BuildType);

            task.BuildType = "Automatic";
            Assert.AreEqual("Automatic", task.BuildType);

            task.BuildType = "Increment";
            Assert.AreEqual("Increment", task.BuildType);

            task.BuildType = "Date";
            Assert.AreEqual("Date", task.BuildType);

            task.BuildType = "DateIncrement";
            Assert.AreEqual("DateIncrement", task.BuildType);

            try { //Internally, BuildType is an enum.
                task.BuildType = "I hate unit tests";
                Assert.Fail("task.BuildType accepted unrecognizd value");
            }
            catch (ArgumentException) { }


            task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;

            Assert.AreEqual("None", task.RevisionType);

            task.RevisionType = string.Empty;
            Assert.AreEqual("None", task.RevisionType);

            task.RevisionType = null;
            Assert.AreEqual("None", task.RevisionType);

            task.RevisionType = "Automatic";
            Assert.AreEqual("Automatic", task.RevisionType);

            task.RevisionType = "Increment";
            Assert.AreEqual("Increment", task.RevisionType);

            task.RevisionType = "NonIncrement";
            Assert.AreEqual("NonIncrement", task.RevisionType);

            try { //Internally, BuildType is an enum.
                task.RevisionType = "I hate unit tests";
                Assert.Fail("task.RevisionType accepted unrecognizd value");
            }
            catch (ArgumentException) { }


            task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            DateTime startDate = new DateTime(2002, 12, 5);
            int daysSinceStartDate = DateTime.Now.Subtract(startDate).Days;

            task.StartDate = startDate.ToString();
            task.BuildType = "DateIncrement";
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(task.Build, daysSinceStartDate);

            int revision = task.Revision;

            task = new Version();
            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            task.StartDate = startDate.ToString();
            task.BuildType = "DateIncrement";
            task.RevisionType = "Increment";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(task.Build, daysSinceStartDate);
            Assert.AreEqual(task.Revision, revision + 1);

            revision = task.Revision;

            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            task.StartDate = startDate.ToString();
            task.BuildType = "DateIncrement";
            task.RevisionType = "None";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(task.Build, daysSinceStartDate);
            Assert.AreEqual(task.Revision, revision);

            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            task.StartDate = startDate.ToString();
            task.BuildType = "DateIncrement";
            task.RevisionType = "Automatic";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(task.Build, daysSinceStartDate);

            revision = (int)DateTime.Now.TimeOfDay.TotalSeconds ;

            task.BuildEngine = new MockBuild();
            task.VersionFile = versionFile;
            task.StartDate = startDate.ToString();
            task.BuildType = "DateIncrement";
            task.RevisionType = "NonIncrement";
            Assert.IsTrue(task.Execute(), "Execute Failed");
            Assert.AreEqual(task.Build, daysSinceStartDate);
            Assert.IsTrue(revision <= task.Revision);


		}
    }
}
