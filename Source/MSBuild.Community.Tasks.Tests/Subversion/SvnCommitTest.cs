// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using MSBuild.Community.Tasks.Subversion;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnCommitTest
    /// </summary>
    [TestFixture]
    public class SvnCommitTest
    {
        public SvnCommitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SvnCommitExecute()
        {
            SvnCommit commit = new SvnCommit();
            commit.BuildEngine = new MockBuild();
        }
    }
}
