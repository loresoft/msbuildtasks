using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MSBuild.Community.Tasks.Sandcastle;
using System.IO;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Sandcastle
{
    [TestFixture()]
    public class XslTransformTest
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
            SandcastleEnviroment sandcastle = new SandcastleEnviroment();

            XslTransform task = new XslTransform();
            task.BuildEngine = builder;
            task.OutputFile = new TaskItem("reflection.xml");
            
            task.XsltFiles = TaskUtility.StringArrayToItemArray(
                Path.Combine(sandcastle.TransformsDirectory, "ApplyVSDocModel.xsl"),
                Path.Combine(sandcastle.TransformsDirectory, "AddFriendlyFilenames.xsl"));

            task.XmlFile = new TaskItem("reflection.org");

            task.Arguments = new string[] {
                "IncludeAllMembersTopic=true", 
                "IncludeInheritedOverloadTopics=true"
            };
            
            bool result = task.Execute();

            Assert.IsTrue(result);
        }
    }
}
