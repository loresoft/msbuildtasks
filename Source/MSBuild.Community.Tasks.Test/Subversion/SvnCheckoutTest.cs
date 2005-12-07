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
    /// Summary description for SvnCheckoutTest
    /// </summary>
    [TestFixture]
    public class SvnCheckoutTest
    {
        public SvnCheckoutTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SvnCheckoutExecute()
        {
            SvnCheckout checkout = new SvnCheckout();
            checkout.BuildEngine = new MockBuild();

            Assert.IsNotNull(checkout);

            checkout.LocalPath = @"Test\Checkout";
            checkout.RepositoryPath = "file:///d:/svn/repo/Test/trunk";
            bool result = checkout.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(checkout.Revision > 0);
        }
    }
}
