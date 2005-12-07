// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using MSBuild.Community.Tasks.Subversion;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnExportTest
    /// </summary>
    [TestFixture]
    public class SvnExportTest
    {
        public SvnExportTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SvnExporeExecute()
        {
            if (Directory.Exists("Export"))
                Directory.Delete("Export", true);
            
            SvnExport export = new SvnExport();
            export.BuildEngine = new MockBuild();

            Assert.IsNotNull(export);

            export.LocalPath = @"Export";
            export.RepositoryPath = "file:///d:/svn/repo/Test/trunk";
            bool result = export.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(export.Revision > 0);
        }
    }
}
