// $Id$

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for NUnitTest
    /// </summary>
    [TestFixture]
    public class NUnitTest
    {
        public NUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void NUnitExecute()
        {
            string[] assemblies = new string[] { @"C:\Program Files\NUnit 2.2.3\bin\nunit.framework.tests.dll" };
            TaskItem[] items = TaskUtility.StringArrayToItemArray(assemblies);

            NUnit task = new NUnit();
            task.BuildEngine = new MockBuild();            
            task.Assemblies = items;
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
