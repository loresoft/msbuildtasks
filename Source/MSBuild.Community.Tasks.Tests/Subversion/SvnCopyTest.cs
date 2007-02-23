// $Id$

using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnCopyTest
    /// </summary>
    [TestFixture]
    public class SvnCopyTest
    {
        public SvnCopyTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test(Description = "Test SVN Copy to make sure it executes")]
        public void SvnCopyExecute()
        {
            SvnCopy copy = new SvnCopy();
            copy.BuildEngine = new MockBuild();
        }
    }
}
