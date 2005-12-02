// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSBuild.Community.Tasks.Subversion;

namespace MSBuild.Community.Tasks.Test.Subversion
{
    /// <summary>
    /// Summary description for SvnCheckoutTest
    /// </summary>
    [TestClass]
    public class SvnCheckoutTest
    {
        public SvnCheckoutTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
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
