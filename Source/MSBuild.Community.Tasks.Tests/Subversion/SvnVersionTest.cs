// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using MSBuild.Community.Tasks.Subversion;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnVersionTest
    /// </summary>
    [TestFixture]
    public class SvnVersionTest
    {
        public SvnVersionTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SvnVersionExecute()
        {
            SvnVersion task = new SvnVersion();
            task.BuildEngine = new MockBuild();
            task.LocalPath = @"..\..\..\";
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.IsTrue(task.Revision > 0, "Invalid Revision Number");
        }
    }
}
