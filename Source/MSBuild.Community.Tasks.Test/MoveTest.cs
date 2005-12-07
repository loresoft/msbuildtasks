// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for MoveTest
    /// </summary>
    [TestFixture]
    public class MoveTest
    {
        public MoveTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void MoveExecute()
        {
            Move task = new Move();
            task.BuildEngine = new MockBuild();
        }
    }
}
