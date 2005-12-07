// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for NDocTest
    /// </summary>
    [TestFixture]
    public class NDocTest
    {
        public NDocTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void NDocExecute()
        {
            NDoc task = new NDoc();
            task.BuildEngine = new MockBuild();
            task.ProjectFilePath = @"..\..\..\..\Documentation\MSBuild.Community.Tasks.ndoc";
            task.Documenter = "MSDN";
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
