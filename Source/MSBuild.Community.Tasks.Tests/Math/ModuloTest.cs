

using System;
using NUnit.Framework;
using MSBuild.Community.Tasks.Math;

namespace MSBuild.Community.Tasks.Tests.Math
{
    [TestFixture]
    public class ModuloTest
    {
        [Test]
        public void DivisionWithNoRemainder_ReturnsZero()
        {
            Modulo task = new Modulo();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "12", "4" };
            task.Execute();

            Assert.AreEqual("0", task.Result);
        }

        [Test]
        public void DivisionWithRemainder_ReturnsRemainder()
        {
            Modulo task = new Modulo();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "14", "4" };
            task.Execute();

            Assert.AreEqual("2", task.Result);
        }

        [Test]
        public void ModulusFractionalNumber_ReturnsFractionalNumber()
        {
            Modulo task = new Modulo();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "12", "3.5" };
            task.Execute();

            Assert.AreEqual("1.5", task.Result);
        }

        [Test]
        public void MultipleNumbers_EvaluatesLeftToRight()
        {
            Modulo task = new Modulo();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "27", "10", "3" };
            task.Execute();

            Assert.AreEqual("1", task.Result);
        }

    }
}
