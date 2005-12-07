// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for XmlReadTest
    /// </summary>
    [TestFixture]
    public class XmlUpdateTest
    {
        public XmlUpdateTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void XmlWriteExecute()
        {
            XmlUpdate task = new XmlUpdate();
            task.BuildEngine = new MockBuild();
            
            task.Prefix = "n";
            task.Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
            task.XmlFileName = @"..\..\..\MSBuild.Community.Tasks\Subversion.proj";
            task.XPath = "/n:Project/n:PropertyGroup/n:LastUpdate";
            task.Value = DateTime.Now.ToLongDateString();
            
            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
