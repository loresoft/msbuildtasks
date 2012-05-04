

using System;
using System.Text;
using MSBuild.Community.Tasks.Math;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Math
{
    /// <summary>
    /// Summary description for SubtractTest
    /// </summary>
    [TestFixture]
    public class SubtractTest
    {
        public SubtractTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void SubtractExecute()
        {
            Subtract task = new Subtract();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "5", "3" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("2", task.Result);

            task = new Subtract();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "1.1", "2.2" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("-1.1", task.Result);
        }
    }
}
