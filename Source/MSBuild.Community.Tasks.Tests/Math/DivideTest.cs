

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

        [Test]
        public void TruncateResult_DoesNotProduceRemainder()
        {
            Divide task = new Divide();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "12", "4" };
            task.TruncateResult = true;
            task.Execute();

            Assert.AreEqual("3", task.Result);
        }

        [Test]
        public void TruncateResult_ProducesRemainder()
        {
            Divide task = new Divide();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "18", "4" };
            task.TruncateResult = true;
            task.Execute();

            Assert.AreEqual("4", task.Result);
        }


        [Test]
        public void TruncateResult_NegativeOperandsProduceRemainder()
        {
            Divide task = new Divide();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "-7", "3" };
            task.TruncateResult = true;
            task.Execute();
            
            Assert.AreEqual("-2", task.Result);
        }

        [Test]
        public void TruncateResult_DecimalOperands()
        {
            Divide task = new Divide();
            task.BuildEngine = new MockBuild();
            task.Numbers = new string[] { "5.4", "1.3" };
            task.TruncateResult = true;
            task.Execute();

            Assert.AreEqual("4", task.Result);
        }
    }
}
