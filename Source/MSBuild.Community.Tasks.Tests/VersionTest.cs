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
		}
    }
}
