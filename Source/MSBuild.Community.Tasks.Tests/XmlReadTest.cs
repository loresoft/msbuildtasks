

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for XmlReadTest
    /// </summary>
    [TestFixture]
    public class XmlReadTest
    {
        [Test(Description="Read XPath information from XML file")]
        public void XmlReadExecute()
        {
            XmlRead task = new XmlRead();
            task.BuildEngine = new MockBuild();
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.XmlFileName = Path.Combine(prjRootPath, @"Source\Subversion.proj");
            task.XPath = "string(/n:Project/n:PropertyGroup/n:MSBuildCommunityTasksPath/text())";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.Prefix = "n";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            task.XPath = "/n:Project/n:Target/@Name";
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }

        [Test]
        public void XPathThatReturnsNumericValue()
        {
            XmlRead task = new XmlRead();
            task.BuildEngine = new MockBuild();
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.XmlFileName = Path.Combine(prjRootPath, @"Source\Subversion.proj");
            task.XPath = "count(/n:Project/n:PropertyGroup/*)";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.Prefix = "n";
            Assert.IsTrue(task.Execute(), "Execute failed.");
            Assert.AreEqual("6", task.Value);
        }

        [Test]
        public void XPathThatReturnsBooleanValue()
        {
            XmlRead task = new XmlRead();
            task.BuildEngine = new MockBuild();
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.XmlFileName = Path.Combine(prjRootPath, @"Source\Subversion.proj");
            task.XPath = "10 < 10";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.Prefix = "n";
            Assert.IsTrue(task.Execute(), "Execute failed.");
            Assert.AreEqual("False", task.Value);
        }

    }
}
