using System;
using System.Xml.XPath;
using MSBuild.Community.Tasks.Properties;
using MSBuild.Community.Tasks.Xml;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture]
    public class MergeTest
    {
        [Test]
        public void LogOnNoSourceFilesTest()
        {
            var task = new Merge();
            var mockBuild = new MockBuild();
            task.BuildEngine = mockBuild;
            var result = task.Execute();

            Assert.IsTrue(result);
            Assert.That(mockBuild.MessageLog, Is.EqualTo(Resources.MergeCompleteNoSourceFiles + Environment.NewLine));
        }
    }
}
