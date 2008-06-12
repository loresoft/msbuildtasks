using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    [TestFixture()]
    public class GacUtilTest
    {
        [SetUp()]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown()]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test()]
        public void Install()
        {
            GacUtil task = new GacUtil();
            task.BuildEngine = new MockBuild();
            task.Command = "Install";
            task.IncludeRelatedFiles = true;
            task.Assemblies = new string[] { @"..\..\..\MSBuild.Community.Tasks\bin\Debug\MSBuild.Community.Tasks.dll" };
            bool result = task.Execute();

            Assert.IsTrue(result);
            
        }

        [Test()]
        public void Uninstall()
        {
            GacUtil task = new GacUtil();
            task.BuildEngine = new MockBuild();
            task.Command = "Uninstall";
            task.Assemblies = new string[] { @"MSBuild.Community.Tasks, Version=1.4.0.0, Culture=neutral, PublicKeyToken=e8bf2261941c3948" };
            bool result = task.Execute();

            Assert.IsTrue(result);

        }
    }
}
