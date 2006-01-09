// $Id$

using System;
using System.Text;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

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
