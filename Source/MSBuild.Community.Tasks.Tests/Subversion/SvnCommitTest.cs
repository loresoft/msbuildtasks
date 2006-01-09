// $Id$

using System;
using System.Text;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

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
