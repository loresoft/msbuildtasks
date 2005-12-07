// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for UnzipTest
    /// </summary>
    [TestFixture]
    public class UnzipTest
    {
        public UnzipTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void UnzipExecute()
        {
            ZipTest zip = new ZipTest();
            zip.ZipExecute();

            Unzip task = new Unzip();
            task.BuildEngine = new MockBuild();
            task.ZipFileName = @"MSBuild.Community.Tasks.zip";
            task.TargetDirectory = @"Backup";

            Assert.IsTrue(task.Execute(), "Execute Failed");

        }
    }
}
