// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using MSBuild.Community.Tasks.Subversion;

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
