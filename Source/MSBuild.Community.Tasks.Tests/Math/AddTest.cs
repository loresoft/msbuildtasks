

using System;
using System.Text;
using MSBuild.Community.Tasks.Math;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.Math
{
    /// <summary>
    /// Summary description for AddTest
    /// </summary>
    [TestFixture]
    public class AddTest
    {
        public AddTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void AddExecute()
        {
            Add task = new Add();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "1", "1" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("2", task.Result);

            task = new Add();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "1.1", "2.1" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("3.2", task.Result);


            task = new Add();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "5", "6", "4" };
            Assert.IsTrue(task.Execute(), "Execute Failed");

            Assert.AreEqual("15", task.Result);


        }
    }
}
