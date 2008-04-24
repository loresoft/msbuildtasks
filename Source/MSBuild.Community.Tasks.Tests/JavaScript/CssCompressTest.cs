using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.JavaScript;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests.JavaScript
{
    [TestFixture()]
    public class CssCompressTest
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
            string cssFile = @"C:\Program Files\Sandcastle\Presentation\vs2005\Styles\Presentation.css";
            MockBuild build = new MockBuild();
            
            CssCompress css = new CssCompress();
            css.BuildEngine = build;
            css.SourceFiles = TaskUtility.StringArrayToItemArray(cssFile);
            css.DestinationFolder = new TaskItem(TaskUtility.TestDirectory);

            bool result = css.Execute();

            Assert.IsTrue(result);
        }
    }
}
