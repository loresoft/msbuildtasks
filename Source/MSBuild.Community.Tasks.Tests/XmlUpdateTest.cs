// $Id$

using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for XmlUpdateTest
    /// </summary>
    [TestFixture]
    public class XmlUpdateTest
    {
        [Test(Description="Update an XML file with XPath navigation")]
        public void XmlUpdateExecute()
        {
            XmlUpdate task = new XmlUpdate();
            task.BuildEngine = new MockBuild();
            
            task.Prefix = "n";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            string prjRootPath = TaskUtility.getProjectRootDirectory(true);
            task.XmlFileName = Path.Combine(prjRootPath, @"Source\MSBuild.Community.Tasks\Subversion.proj");
            task.XPath = "/n:Project/n:PropertyGroup/n:LastUpdate";
            task.Value = DateTime.Now.ToLongDateString();
            
            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
