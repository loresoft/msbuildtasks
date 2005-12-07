// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using MSBuild.Community.Tasks.Subversion;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnUpdateTest
    /// </summary>
    [TestFixture]
    public class SvnUpdateTest
    {
        public SvnUpdateTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SvnUpdateExecute()
        {
            SvnUpdate task = new SvnUpdate();
            task.BuildEngine = new MockBuild();

        }
    }
}
