// $Id$

using System;
using System.Text;
using MSBuild.Community.Tasks.Math;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Math
{
    /// <summary>
    /// Summary description for DivideTest
    /// </summary>
    [TestFixture]
    public class DivideTest
    {
        public DivideTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void DivideExecute()
        {
            Divide task = new Divide();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "12", "4" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("3", task.Result);

            task = new Divide();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "1", "2" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("0.5", task.Result);

        }
    }
}
