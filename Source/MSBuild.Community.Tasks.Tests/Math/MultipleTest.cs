

using System;
using System.Text;
using MSBuild.Community.Tasks.Math;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Math
{
    /// <summary>
    /// Summary description for MultipleTest
    /// </summary>
    [TestFixture]
    public class MultipleTest
    {
        public MultipleTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void MultipleExecute()
        {
            Multiple task = new Multiple();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "3", "4" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("12", task.Result);

            task = new Multiple();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "1.1", "2.1" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("2.31", task.Result);

            task = new Multiple();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "5", "6", "4" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("120", task.Result);
        }
    }
}
