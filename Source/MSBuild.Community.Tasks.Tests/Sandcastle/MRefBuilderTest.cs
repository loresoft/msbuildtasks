using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.Sandcastle;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Sandcastle
{
    [TestFixture()]
    public class MRefBuilderTest
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
        public void Execute()
        {
            MockBuild builder = new MockBuild();
            
            MRefBuilder task = new MRefBuilder();
            task.BuildEngine = builder;
            task.Assemblies = new TaskItem[] {
                new TaskItem(@"..\..\..\..\Libraries\ICSharpCode.SharpZipLib.dll") 
            };
            task.OutputFile = new TaskItem("reflection.org");
            bool result = task.Execute();

            Assert.IsTrue(result);
        }
    }
}
