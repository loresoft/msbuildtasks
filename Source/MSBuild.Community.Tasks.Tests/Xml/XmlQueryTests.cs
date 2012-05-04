
using System;
using NUnit.Framework;
using MSBuild.Community.Tasks.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks.Tests.Xml
{
    [TestFixture]
    public class XmlQueryTests
    {
        XmlQuery task;

        public void setupTask(string xml)
        {
            task = new XmlQuery();
            task.BuildEngine = new MockBuild();
            task.Lines = new ITaskItem[] { new TaskItem(@"<?xml version=""1.0"" encoding=""utf-8"" ?>"), new TaskItem(xml) };
        }

        [Test]
        public void RetrieveAttributeValueViaXPath()
        {
            setupTask(testXml);
            task.XPath = "/configuration/system.web/compilation/@debug";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual("true", task.Values[0].ToString());
        }

        [Test]
        public void RetrieveAttributeValuesViaMetadata()
        {
            setupTask(testXml);
            task.XPath = "/configuration/system.web/compilation";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual("c#", task.Values[0].GetMetadata("defaultLanguage"));
            Assert.AreEqual("true", task.Values[0].GetMetadata("debug"));
        }


        [Test]
        public void RetrieveElementValue()
        {
            setupTask(testXml);
            task.XPath = "/configuration/singleValue/LastName";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(1, task.ValuesCount);
            Assert.AreEqual("Doe", task.Values[0].GetMetadata("_innerXml"));
        }

        [Test]
        public void RetrieveSetOfElements()
        {
            setupTask(testXml);
            task.XPath = "/configuration/appSettings/add";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(3, task.ValuesCount);
        }

        [Test]
        public void RetrieveSpecificElementInSetViaXPath()
        {
            setupTask(testXml);
            task.XPath = "/configuration/appSettings/add[@key='MaxRowsPerPage']";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(1, task.ValuesCount);
            Assert.AreEqual("25", task.Values[0].GetMetadata("value"));
        }

        [Test]
        public void RetrieveSpecificElementInSetViaIndexer()
        {
            setupTask(testXml);
            task.XPath = "/configuration/appSettings/add";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(3, task.ValuesCount);
            Assert.AreEqual("10", task.Values[2].GetMetadata("value"));
        }

        [Test]
        public void QueryXmlWithMultipleNamespaces()
        {
            setupTask(textXmlWithNS);
            task.XPath = "/x:transform/x:template/soap:Header/o:CustomType[@o:Id='B']";
            task.NamespaceDefinitions = new ITaskItem[] { 
                new TaskItem("o=urn:test:msbuild:community"), 
                new TaskItem("soap=http://www.w3.org/2001/12/soap-envelope"),
                new TaskItem("x=http://www.w3.org/1999/XSL/Transform") 
            };
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(1, task.ValuesCount);
            Assert.AreEqual("Gibberish", task.Values[0].GetMetadata("_value"));
        }

        [Test]
        public void RetrieveElementValueWithCustomReservedMetaDataPrefix()
        {
            setupTask(testXml);
            task.XPath = "/configuration/singleValue/LastName";
            task.ReservedMetaDataPrefix = "*";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(1, task.ValuesCount);
            Assert.AreEqual("Doe", task.Values[0].GetMetadata("*innerXml"));
        }

        [Test]
        public void QueryReturnsBooleanValue()
        {
            setupTask(testXml);
            task.XPath = "10 < 10";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(false.ToString(), task.Values[0].ToString());

            setupTask(testXml);
            task.XPath = "10 = 10";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual(true.ToString(), task.Values[0].ToString());
        }

        [Test]
        public void XPathThatReturnsNumericValue()
        {
            setupTask(testXml);
            task.XPath = "count(/configuration/appSettings/*)";
            Assert.IsTrue(task.Execute(), "Should have executed successfully");
            Assert.AreEqual("3", task.Values[0].ToString(), "Should have found 3 children of the appSettings node.");
        }

        [Test]
        public void LoadXmlFromFile()
        {
            task = new XmlQuery();
            task.BuildEngine = new MockBuild();

            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.XmlFileName = System.IO.Path.Combine(prjRootPath, @"Source\Subversion.proj");
            task.XPath = "count(/n:Project/n:PropertyGroup/*)";
            task.NamespaceDefinitions = new ITaskItem[] { 
                new TaskItem("n=http://schemas.microsoft.com/developer/msbuild/2003")
            };

            Assert.IsTrue(task.Execute(), "Should have executed successfully.");
            Assert.AreEqual("6", task.Values[0].ToString());

        }

        [Test]
        public void SpecifyBothLinesAndXmlFileName_ReturnFalse()
        {
            setupTask(testXml);
            string prjRootPath = TaskUtility.GetProjectRootDirectory(true);
            task.XmlFileName = System.IO.Path.Combine(prjRootPath, @"Source\Subversion.proj");
            task.XPath = "count(/configuration/appSettings/*)";
            Assert.IsFalse(task.Execute(), "Should have failed to execute.");
        }

        [Test]
        public void SpecifyNeitherLinesNorXmlFileName_ReturnFalse()
        {
            task = new XmlQuery();
            task.BuildEngine = new MockBuild();
            task.XPath = "count(/configuration/appSettings/*)";
            Assert.IsFalse(task.Execute(), "Should have failed to execute.");
        }


        string testXml = @"
<configuration>
  <configSections>
	  <section name=""singleValue"" type=""SingleValueSection"" />
  </configSections>
  <appSettings>
    <add key=""DefaultTheme"" value=""Simple"" />
    <add key=""MaxRowsPerPage"" value=""25"" />
    <!-- This is a comment -->
    <add key=""MinutesToCacheResults"" value=""10"" />
  </appSettings>
  <singleValue>
    <FirstName>John</FirstName>
    <LastName>Doe</LastName>
  </singleValue>
  <system.web>
    <compilation defaultLanguage=""c#"" debug=""true"" />
    <authorization>
      <deny users=""?"" />        
      <allow users=""*"" />
    </authorization>
    <trace enabled=""false"" />
  </system.web>
</configuration>
";
        string textXmlWithNS = @"
<xsl:transform version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:soap=""http://www.w3.org/2001/12/soap-envelope"" xmlns:other=""urn:test:msbuild:community"">
  <xsl:template match=""/"">
    <soap:Header>
      <other:CustomType other:Id=""A""  soap:mustUnderstand=""1"">Nonsense</other:CustomType>
      <other:CustomType other:Id=""B"">Gibberish</other:CustomType>
    </soap:Header>
  </xsl:template>
</xsl:transform>
";

    }
}
