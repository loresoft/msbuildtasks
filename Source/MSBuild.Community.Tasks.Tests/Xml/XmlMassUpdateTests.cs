
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
        public void UpdateKeyedElementsWithoutCopyingUpdateControlNamespaceDeclaration()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentXml = contentXmlWithMultipleItems; 
            task.SubstitutionsFile = new TaskItem("test2.xml");
            task.SubstitutionsXml = substitutionsXmlWithUpdateControlNamespace;
            string originalB = getInitialValue("/configuration/appSettings/add[@key='B']/@value");

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");

            // Make sure update succeeded
            Assert.AreNotEqual("40", originalB, "Make sure the original value is different from the value it will be set to.");
            assertXml("40", "/configuration/appSettings/add[@key='B']/@value", "B should have changed");
            // Make sure namespace declaration was not copied

            XmlNode configurationNode = task.MergedXmlDocument.SelectSingleNode("/configuration");
            Assert.IsNotNull(configurationNode, "The configuration node should exist.");
            Assert.AreEqual(0, configurationNode.Attributes.Count, "The configuration node should not have any attributes.");
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

        [Test]
        public void RemoveElement()
        {
            setupTask();
            setupContent();
            task.SubstitutionsRoot = "/configuration/substitutions/removeElement";

            XmlNode traceNode = getNode(task.ContentXml, "/configuration/system.web/trace");
            Assert.IsNotNull(traceNode, "The trace element should exist.");

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");

            traceNode = getNode(task.MergedXmlDocument, "/configuration/system.web/trace");
            Assert.IsNull(traceNode, "The trace element should not exist.");
        }

        [Test]
        public void RemoveKeyedElement()
        {
            setupTask();
            setupContent();
            task.SubstitutionsRoot = "/configuration/substitutions/removeKeyedElement";

            string originalA = getInitialValue("/configuration/appSettings/add[@key='A']/@value");
            string originalB = getInitialValue("/configuration/appSettings/add[@key='B']/@value");
            string originalC = getInitialValue("/configuration/appSettings/add[@key='C']/@value");
            Assert.IsNotNull(originalA, "A should exist.");
            Assert.IsNotNull(originalB, "B should exist.");
            Assert.IsNotNull(originalC, "C should exist.");

            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");

            assertXml(originalA, "/configuration/appSettings/add[@key='A']/@value", "A should not have changed");
            assertXml(originalC, "/configuration/appSettings/add[@key='C']/@value", "C should not have changed");

            XmlNode bNode = getNode(task.MergedXmlDocument, "/configuration/appSettings/add[@key='B']/@value");
            Assert.IsNull(bNode, "The B element should not exist.");
        }

        [Test]
        public void UpdateNodeValue()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/configuration/substitutions/prod";
            task.ContentXml = contentXmlWithNodeValues;
            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");
            assertXml("unchanged", "/configuration/applicationSettings/ExampleProject.Settings/setting[@name='id']/value/text()", "Should not have changed unmentioned node.");
            assertXml("after change", "/configuration/applicationSettings/ExampleProject.Settings/setting[@name='key']/value/text()", "Should have changed node text");
        }

        [Test]
        public void UpdateCDataValue()
        {
            setupTask();
            task.ContentFile = new TaskItem("test.xml");
            task.ContentRoot = "/configuration";
            task.SubstitutionsRoot = "/configuration/substitutions/stage";
            task.ContentXml = contentXmlWithNodeValues;
            bool executeSucceeded = task.Execute();
            Assert.IsTrue(executeSucceeded, "Task should have succeeded.");
            assertXml(" if (x > 20) {return y*2;} ", "/configuration/applicationSettings/ExampleProject.Settings/code/text()", "Should have changed CDATA.");
        }

        private void setupContent()
        {
            task.ContentFile = new TaskItem("test.xml");
            task.ContentXml = contentXmlWithMultipleItems;
            task.ContentRoot = "/configuration";
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

        private XmlNode getNode(string xml, string xpath)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            return getNode(document, xpath);
        }
        private XmlNode getNode(XmlDocument document, string xpath)
        {
            return document.SelectSingleNode(xpath);
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
    <removeElement>
      <system.web>
        <trace xmu:action=""remove"" />
      </system.web>
    </removeElement>
    <removeKeyedElement>
      <appSettings>
        <add xmu:key=""key"" key=""B"" xmu:action=""remove"" />
      </appSettings>
    </removeKeyedElement>
</substitutions>
</configuration>";

        const string contentXmlWithNodeValues = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <applicationSettings>
    <ExampleProject.Settings>
      <setting name=""id"" serializeAs=""String"">
        <value>unchanged</value>
      </setting>    
      <setting name=""key"" serializeAs=""String"">
        <value>before change</value>
      </setting>
      <code>
        <![CDATA[ if (x > 3) {return y;} ]]> 
      </code>
    </ExampleProject.Settings>
  </applicationSettings>
  <substitutions xmlns:xmu=""urn:msbuildcommunitytasks-xmlmassupdate"">
    <prod>
      <applicationSettings>
        <ExampleProject.Settings>
          <setting xmu:key=""name"" name=""key"" serializeAs=""String"">
            <value>after change</value>
          </setting>
        </ExampleProject.Settings>
      </applicationSettings>
    </prod>
    <stage>
      <applicationSettings>
        <ExampleProject.Settings>
          <code>
            <![CDATA[ if (x > 20) {return y*2;} ]]> 
          </code>
        </ExampleProject.Settings>
      </applicationSettings>
    </stage>
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
        private const string substitutionsXmlWithUpdateControlNamespace = @"<configuration xmlns:xmu=""urn:msbuildcommunitytasks-xmlmassupdate"">
  <appSettings>
    <add xmu:key=""key"" key=""B"" value=""40"" />
    <add xmu:key=""key"" key=""C"" value=""60"" />
  </appSettings>
</configuration>";
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
