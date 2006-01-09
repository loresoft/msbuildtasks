// $Id$

using System;
using System.Text;
using MSBuild.Community.Tasks.Subversion;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Subversion
{
    /// <summary>
    /// Summary description for SvnClientTest
    /// </summary>
    [TestFixture]
    public class SvnClientTest
    {
        public SvnClientTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SvnClientExecute()
        {
            SvnClient client = new SvnClient();
            client.BuildEngine = new MockBuild();
        }
    }
}
