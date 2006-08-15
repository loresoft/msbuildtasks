// $Id$
using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Xml;

namespace MSBuild.Community.Tasks.Xml
{
    /// <summary>
    /// Performs multiple updates on an XML file
    /// </summary>
    public class XmlMassUpdate : Task
    {
        private ITaskItem contentFile;

        /// <summary>
        /// The original file whose content is to be updated
        /// </summary>
        /// <remarks>This task is currently under construction and not necessarily feature complete.</remarks>
        [Required]
        public ITaskItem ContentFile
        {
            get { return contentFile; }
            set { contentFile = value; }
        }

        private ITaskItem substitutionsFile;

        /// <summary>
        /// The file containing the list of updates to perform
        /// </summary>
        public ITaskItem SubstitutionsFile
        {
            get { return substitutionsFile; }
            set { substitutionsFile = value; }
        }

        private ITaskItem mergedFile;

        /// <summary>
        /// The file created after performing the updates
        /// </summary>
        public ITaskItem MergedFile
        {
            get { return mergedFile; }
            set { mergedFile = value; }
        }


        private string substitutionsRoot;

        /// <summary>
        /// The XPath expression used to locate the list of substitutions to perform
        /// </summary>
        [Required]
        public string SubstitutionsRoot
        {
            get { return substitutionsRoot; }
            set { substitutionsRoot = value; }
        }

        private string contentRoot;

        /// <summary>
        /// The XPath expression identifying root node that substitions are relative to
        /// </summary>
        [Required]
        public string ContentRoot
        {
            get { return contentRoot; }
            set { contentRoot = value; }
        }

        /// <summary>
        /// A collection of prefix=namespace definitions used to query the XML documents
        /// </summary>
        /// <example>Defining multiple namespaces:
        /// <code><![CDATA[
        /// <XmlQuery Lines="@(FileContents)"
        ///	    XPath = "/x:transform/x:template/soap:Header"
        /// 	NamespaceDefinitions = "soap=http://www.w3.org/2001/12/soap-envelope;x=http://www.w3.org/1999/XSL/Transform">
        /// 	<Output TaskParameter="Values" ItemName="SoapEnvelopeNode" />
        /// </XmlQuery>]]></code>
        /// </example>
        public ITaskItem[] NamespaceDefinitions
        {
            get { return namespaceDefinitions; }
            set { namespaceDefinitions = value; }
        }
        private ITaskItem[] namespaceDefinitions;
        XmlNamespaceManager namespaceManager;

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// True if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            if (substitutionsFile == null) substitutionsFile = contentFile;
            if (mergedFile == null) mergedFile = contentFile;

            string existingFilePath = contentFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(existingFilePath)) existingFilePath = contentFile.ItemSpec;

            string substitutionsFilePath = substitutionsFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(substitutionsFilePath)) substitutionsFilePath = substitutionsFile.ItemSpec;

            XmlDocument existingDocument = new XmlDocument();
            existingDocument.Load(existingFilePath);

            XmlDocument substitutionsDocument;
            if (String.Compare(existingFilePath, substitutionsFilePath, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                substitutionsDocument = existingDocument;
            }
            else
            {
                substitutionsDocument = new XmlDocument();
                substitutionsDocument.Load(substitutionsFilePath);
            }
            namespaceManager = new XmlNamespaceManager(existingDocument.NameTable);
            XmlTaskHelper.LoadNamespaceDefinitionItems(namespaceManager, namespaceDefinitions);

            XmlNode substitutionsRootNode = substitutionsDocument.SelectSingleNode(substitutionsRoot, namespaceManager);
            XmlNode configurationRootNode = existingDocument.SelectSingleNode(contentRoot, namespaceManager);

            addAllChildNodes(existingDocument, substitutionsRootNode, configurationRootNode);

            string mergedFilePath = mergedFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(mergedFilePath)) mergedFilePath = mergedFile.ItemSpec;

            existingDocument.Save(mergedFilePath);
            return true;
        }

        private void addAllChildNodes(XmlDocument config, XmlNode sourceParentNode, XmlNode configurationParentNode)
        {
            XmlNode sourceNode = sourceParentNode.FirstChild;
            while (sourceNode != null && sourceNode.NodeType == XmlNodeType.Element)
            {
                XmlNode targetNode = ensureNode(config, configurationParentNode, sourceNode.Name);

                foreach (XmlAttribute sourceAttribute in sourceNode.Attributes)
                {
                    setAttributeValue(config, targetNode, sourceAttribute.Name, sourceAttribute.Value);
                }

                addAllChildNodes(config, sourceNode, targetNode);
                sourceNode = sourceNode.NextSibling;
            }
        }

        private void setAttributeValue(XmlDocument config, XmlNode modifiedNode, string attributeName, string attributeValue)
        {
            XmlAttribute targetAttribute = modifiedNode.Attributes[attributeName];
            if (targetAttribute == null)
            {
                Log.LogMessage(MessageImportance.Low, "Creating attribute {0} on {1}", attributeName, modifiedNode.Name);
                targetAttribute = modifiedNode.Attributes.Append(config.CreateAttribute(attributeName));
            }
            targetAttribute.Value = attributeValue;
            Log.LogMessage("Setting attribute {0} to {1} on {2}", targetAttribute.Name, targetAttribute.Value, modifiedNode.Name);
        }

        private XmlNode ensureNode(XmlDocument config, XmlNode parentNode, string nodeName)
        {
            XmlNode targetNode = parentNode.SelectSingleNode(nodeName, namespaceManager);
            if (targetNode == null)
            {
                Log.LogMessage(MessageImportance.Low, "Creating node {0} under {1}", nodeName, parentNode.Name);
                targetNode = parentNode.AppendChild(config.CreateNode(XmlNodeType.Element, nodeName, String.Empty));
            }
            return targetNode;
        }
    }
}
