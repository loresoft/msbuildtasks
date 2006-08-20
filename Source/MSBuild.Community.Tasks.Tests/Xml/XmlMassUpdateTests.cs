// $Id$
using System;
using NUnit.Framework;
using MSBuild.Community.Tasks.Xml;
using System.Xml;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace MSBuild.Community.Tasks.Tests.Xml
{
    [TestFixture]
    public class XmlMassUpdateTests
    {
        XmlMassUpdateTestWrapper task;

        public void setupTask()
        {
            task = new XmlMassUpdateTestWrapper();
            task.BuildEngine = new MockBuild();
        }

        [Test]
        public void Execute()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/configuration/substitutions/prod";
            task.ContentXml = contentXml;

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");
        }

        [Test]
        public void SpecifyContentFileOnly_SubstitionsAndMergedShouldBeContentFile()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/configuration/substitutions/prod";
            task.ContentXml = contentXml;
            
            bool executeSucceeded = task.Execute();

            Assert.AreEqual(task.ContentPathUsedByTask, task.SubstitutionsPathUsedByTask, "Substitutions file should default to the content file.");
            Assert.AreEqual(task.ContentPathUsedByTask, task.MergedPathUsedByTask, "Merged file should default to the content file.");
        }

        [Test]
        public void SpecifyContentFileOnlyWithNoRoots_TaskFails()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentXml = contentXml;

            bool executeSucceeded = task.Execute();

            Assert.IsFalse(executeSucceeded, "Task should have failed - cannot use same root for content and substitution.");
        }

        [Test]
        public void SpecifyContentFileOnlyWithSameRoots_TaskFails()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentXml = contentXml;
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/configuration";
            bool executeSucceeded = task.Execute();

            Assert.IsFalse(executeSucceeded, "Task should have failed - cannot use same root for content and substitution.");
        }


        [Test]
        public void SameContentAndSubstitutionsFiles()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/configuration/substitutions/prod";
            task.ContentXml = contentXml;
            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");
            Assert.AreEqual(task.ContentPathUsedByTask, task.SubstitutionsPathUsedByTask, "Content and Substitutions file should be the same.");
            assertXml("false", "/configuration/system.web/compilation/@debug", "Should have changed debug");
        }

        [Test]
        public void DifferentContentAndSubstitutionsFiles()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.SubstitutionsFile = new TaskItem("test2.xml");
            task.ContentXml = contentXml;
            task.SubstitutionsXml = substitutionsXml;
            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");
            assertXml("VB", "/configuration/system.web/compilation/@defaultLanguage", "Should have changed defaultLangugage");
        }

        [Test]
        public void UpdateDocumentWithNamespaces()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.SubstitutionsFile = new TaskItem("test2.xml");
            task.ContentXml = contentXml;
            task.SubstitutionsXml = substitutionsXmlWithNamespaces;
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/MS:Project/MS:PropertyGroup/MS:Substitutions";
            task.NamespaceDefinitions = new ITaskItem[] { new TaskItem("MS=http://schemas.microsoft.com/developer/msbuild/2003") };

            string originalDebug = getInitialValue("/configuration/system.web/compilation/@debug");
            Assert.AreEqual("true", originalDebug, "Should be true initially.");

            bool executeSucceeded = task.Execute();

            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");
            assertXml("false", "/configuration/system.web/compilation/@debug", "Should have changed debug value");
        }

        private void setupContent()
        {
            task.ContentFile = new TaskItem("test.xml");
            task.ContentXml = contentXmlWithMultipleItems;
            task.ContentRoot = "/configuration";
        }

        [Test]
        public void UpdateKeyedElementOnly()
        {
            setupTask();
            setupContent();
            task.SubstitutionsRoot = "/configuration/substitutions/updateKeyed";

            string originalA = getInitialValue("/configuration/appSettings/add[@key='A']/@value");
            string originalC = getInitialValue("/configuration/appSettings/add[@key='C']/@value");

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");

            // Make sure only one entry is changed
            assertXml(originalA, "/configuration/appSettings/add[@key='A']/@value", "A should not have changed");
            assertXml("100", "/configuration/appSettings/add[@key='B']/@value", "B should have changed");
            assertXml(originalC, "/configuration/appSettings/add[@key='C']/@value", "C should not have changed");
        }


        [Test]
        public void UpdateMultipleKeyedElements()
        {
            setupTask();
            setupContent();
            task.SubstitutionsRoot = "/configuration/substitutions/updateMultipleKeyed";

            string originalB = getInitialValue("/configuration/appSettings/add[@key='B']/@value");

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");

            // Make sure only one entry is changed
            assertXml("Red", "/configuration/appSettings/add[@key='A']/@value", "A should have changed");
            assertXml(originalB, "/configuration/appSettings/add[@key='B']/@value", "B should not have changed");
            assertXml("Green", "/configuration/appSettings/add[@key='C']/@value", "C should have changed");
        }

        [Test]
        public void AddNewKeyedElement()
        {
            setupTask();
            setupContent();
            task.SubstitutionsRoot = "/configuration/substitutions/addNewKeyed";

            string originalA = getInitialValue("/configuration/appSettings/add[@key='A']/@value");
            string originalB = getInitialValue("/configuration/appSettings/add[@key='B']/@value");
            string originalC = getInitialValue("/configuration/appSettings/add[@key='C']/@value");
            string originalD = getInitialValue("/configuration/appSettings/add[@key='D']/@value");
            Assert.IsNull(originalD, "D should not exist yet.");

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");

            // Make sure other entries are untouched, and new entry is created
            assertXml(originalA, "/configuration/appSettings/add[@key='A']/@value", "A should not have changed");
            assertXml(originalB, "/configuration/appSettings/add[@key='B']/@value", "B should not have changed");
            assertXml(originalC, "/configuration/appSettings/add[@key='C']/@value", "C should not have changed");
            assertXml("Earth", "/configuration/appSettings/add[@key='D']/@value", "A should have changed");
        }



        private string getInitialValue(string xpath)
        {
            XmlDocument original = new XmlDocument();
            original.LoadXml(task.ContentXml);
            XmlNode node = original.SelectSingleNode(xpath);
            if (node == null) return null;
            return node.Value;
        }

        private void assertXml(string expectedValue, string xpath, string message)
        {
            Assert.DoAssert(new XPathAsserter(task.MergedXmlDocument, xpath, expectedValue, message));
        }


        const string contentXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <system.web>
    <compilation defaultLanguage=""c#"" debug=""true"" />
  </system.web>
  <substitutions>
    <prod>
      <system.web>
        <compilation debug=""false"" />
      </system.web>
    </prod>
  </substitutions>
</configuration>";

        const string substitutionsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <system.web>
    <compilation defaultLanguage=""VB"" debug=""true"" />
  </system.web>
</configuration>";

        const string contentXmlWithMultipleItems = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""A"" value=""10"" />
    <add key=""B"" value=""20"" />
    <!-- This is a comment -->
    <add key=""C"" value=""30"" />
  </appSettings>
  <singleValue>
    <FirstName>John</FirstName>
    <LastName>Doe</LastName>
  </singleValue>
  <system.web>
    <compilation defaultLanguage=""c#"" debug=""true"" />
    <authorization>
      <deny users=""?"" />    
      <deny roles=""Forbidden"" />    
      <allow users=""*"" />
    </authorization>
    <trace enabled=""false"" pageOutput=""true"" />
  </system.web>

  <substitutions xmlns:xmu=""urn:msbuildcommunitytasks-xmlmassupdate"">
    <updateKeyed>
      <appSettings>
        <add xmu:key=""key"" key=""B"" value=""100"" />
      </appSettings>
    </updateKeyed>
    <updateMultipleKeyed>
      <appSettings>
        <add xmu:key=""key"" key=""A"" value=""Red"" />
        <add xmu:key=""key"" key=""C"" value=""Green"" />
      </appSettings>
    </updateMultipleKeyed>
    <addNewKeyed>
      <appSettings>
        <add xmu:key=""key"" key=""D"" value=""Earth"" />
      </appSettings>
    </addNewKeyed>
</substitutions>
</configuration>";

        const string substitutionsXmlWithNamespaces = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Substitutions>
      <system.web>
        <compilation debug=""false"" />
      </system.web>
    </Substitutions>
  </PropertyGroup>
</Project>
";
    }

    /// <summary>
    /// Used during unit tests to override the dependency on the filesystem.
    /// </summary>
    public class XmlMassUpdateTestWrapper : XmlMassUpdate
    {
        private string contentXml;
        public string ContentXml
        {
            get { return contentXml; }
            set { contentXml = value; }
        }

        private string substitutionsXml;
        public string SubstitutionsXml
        {
            get { return substitutionsXml; }
            set { substitutionsXml = value; }
        }

        private XmlDocument mergedXmlDocument;
        public XmlDocument MergedXmlDocument
        {
            get { return mergedXmlDocument; }
            set { mergedXmlDocument = value; }
        }

        protected override XmlDocument LoadContentDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(contentXml);
            return doc;
        }
        protected override XmlDocument LoadSubstitutionsDocument()
        {
            //TODO: this needs to take into account that substitutions might be same as content
            if (ContentPathUsedByTask.Equals(SubstitutionsPathUsedByTask, StringComparison.InvariantCultureIgnoreCase))
            {
                return LoadContentDocument();
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(substitutionsXml);
            return doc;
        }
        protected override bool SaveMergedDocument(XmlDocument mergedDocument)
        {
            this.mergedXmlDocument = mergedDocument;
            return true;
        }
    }

}
