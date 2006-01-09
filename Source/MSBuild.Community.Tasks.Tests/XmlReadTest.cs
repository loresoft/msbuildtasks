// $Id$

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
            string prjRootPath = TaskUtility.getProjectRootDirectory(true);
            task.XmlFileName = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks\Subversion.proj");
            task.XPath = "string(/n:Project/n:PropertyGroup/n:MSBuildCommunityTasksPath/text())";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.Prefix = "n";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            task.XPath = "/n:Project/n:Target/@Name";
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
