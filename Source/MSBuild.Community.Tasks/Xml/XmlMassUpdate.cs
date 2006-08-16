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
        /// <remarks>When not specified, the default is the document root: <c>/</c>
        /// <para>When there is a set of elements with the same name, and you want to update
        /// a single element which can be identified by one of its attributes, you can mark the attribute
        /// with the following namespace: <c>urn:msbuildcommunitytasks-xmlmassupdate</c>.</para></remarks>
        public string SubstitutionsRoot
        {
            get { return substitutionsRoot; }
            set { substitutionsRoot = value; }
        }

        private string contentRoot;

        /// <summary>
        /// The XPath expression identifying root node that substitions are relative to
        /// </summary>
        /// <remarks>When not specified, the default is the document root: <c>/</c></remarks>
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
        /// <XmlMassUpdate ContentFile="web.config"
        ///	    SubstitutionsRoot="/configuration/substitutions"
        /// 	NamespaceDefinitions = "soap=http://www.w3.org/2001/12/soap-envelope;x=http://www.w3.org/1999/XSL/Transform">
        /// 	/></code>
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

            string contentFilePath = contentFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(contentFilePath)) contentFilePath = contentFile.ItemSpec;

            string substitutionsFilePath = substitutionsFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(substitutionsFilePath)) substitutionsFilePath = substitutionsFile.ItemSpec;

            if (String.IsNullOrEmpty(substitutionsRoot)) substitutionsRoot = "/";
            if (String.IsNullOrEmpty(contentRoot)) contentRoot = "/";
            
            if (contentFilePath.Equals(substitutionsFilePath, StringComparison.InvariantCultureIgnoreCase)  && (contentRoot == substitutionsRoot))
            {
                Log.LogError("The SubstitutionsRoot must be different from the ContentRoot when the ContentFile and SubstitutionsFile are the same.");
                return false;
            }

            if (!System.IO.File.Exists(contentFilePath))
            {
                Log.LogError("Unable to load content file {0}", contentFilePath);
                return false;
            }

            XmlDocument existingDocument = new XmlDocument();
            existingDocument.Load(contentFilePath);

            XmlDocument substitutionsDocument;
            if (contentFilePath.Equals(substitutionsFilePath, StringComparison.InvariantCultureIgnoreCase))
            {
                substitutionsDocument = existingDocument;
            }
            else
            {
                if (!System.IO.File.Exists(substitutionsFilePath))
                {
                    Log.LogError("Unable to load substitutions file {0}", substitutionsFilePath);
                    return false;
                }
                substitutionsDocument = new XmlDocument();
                substitutionsDocument.Load(substitutionsFilePath);
            }
            namespaceManager = new XmlNamespaceManager(existingDocument.NameTable);
            XmlTaskHelper.LoadNamespaceDefinitionItems(namespaceManager, namespaceDefinitions);

            XmlNode substitutionsRootNode = substitutionsDocument.SelectSingleNode(substitutionsRoot, namespaceManager);
            XmlNode configurationRootNode = existingDocument.SelectSingleNode(contentRoot, namespaceManager);

            try
            {
                addAllChildNodes(existingDocument, substitutionsRootNode, configurationRootNode);
            }
            catch (MultipleKeyedAttributesException)
            {
                Log.LogError("A substitution node contained more than one keyed attributed.");
                return false;
            }

            string mergedFilePath = mergedFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(mergedFilePath)) mergedFilePath = mergedFile.ItemSpec;
            try
            {
                existingDocument.Save(mergedFilePath);
            }
            catch (System.IO.IOException exception)
            {
                Log.LogError("Unable to create MergedFile - {0}", exception.Message);
                return false;
            }
            return true;
        }

        private void addAllChildNodes(XmlDocument config, XmlNode sourceParentNode, XmlNode configurationParentNode)
        {
            XmlNode sourceNode = sourceParentNode.FirstChild;
            while (sourceNode != null)
            {
                if (sourceNode.NodeType == XmlNodeType.Element)
                {
                    XmlNode targetNode = ensureNode(config, configurationParentNode, sourceNode);

                    foreach (XmlAttribute sourceAttribute in sourceNode.Attributes)
                    {
                        if (sourceAttribute.NamespaceURI != KeyedNodeIdentifyingNamespace)
                        {
                            setAttributeValue(config, targetNode, sourceAttribute.Name, sourceAttribute.Value);
                        }
                    }

                    addAllChildNodes(config, sourceNode, targetNode);
                }
                sourceNode = sourceNode.NextSibling;
            }
        }

        private void setAttributeValue(XmlDocument config, XmlNode modifiedNode, string attributeName, string attributeValue)
        {
            XmlAttribute targetAttribute = modifiedNode.Attributes[attributeName];
            if (targetAttribute == null)
            {
                Log.LogMessage(MessageImportance.Low, "Creating attribute '{0}' on '{1}'", attributeName, modifiedNode.Name);
                targetAttribute = modifiedNode.Attributes.Append(config.CreateAttribute(attributeName));
            }
            targetAttribute.Value = attributeValue;
            Log.LogMessage("Setting attribute '{0}' to '{1}' on '{2}'", targetAttribute.Name, targetAttribute.Value, modifiedNode.Name);
        }

        /// <summary>
        /// The namespace used to decorate attributes that should be used as a key to locate an existing node.
        /// </summary>
        public readonly string KeyedNodeIdentifyingNamespace = "urn:msbuildcommunitytasks-xmlmassupdate";

        private XmlNode ensureNode(XmlDocument config, XmlNode parentNode, XmlNode sourceNode)
        {
            XmlAttribute keyAttribute = getKeyAttribute(sourceNode);
            string xpath;
            if (keyAttribute == null)
            {
                xpath = sourceNode.Name;
            }
            else
            {
                Log.LogMessage(MessageImportance.Low, "Using keyed attribute '{0}' to locate node '{1}'", keyAttribute.LocalName, sourceNode.LocalName);
                xpath = String.Format("{0}[@{1}='{2}']", sourceNode.LocalName, keyAttribute.LocalName, keyAttribute.Value);
            }
            XmlNode targetNode = parentNode.SelectSingleNode(xpath, namespaceManager);
            if (targetNode == null)
            {
                Log.LogMessage(MessageImportance.Low, "Creating node '{0}' under '{1}'", sourceNode.Name, parentNode.Name);
                targetNode = parentNode.AppendChild(config.CreateNode(XmlNodeType.Element, sourceNode.Name, String.Empty));
                if (keyAttribute != null)
                {
                    XmlAttribute keyAttributeOnNewNode = targetNode.Attributes.Append(config.CreateAttribute(keyAttribute.LocalName));
                    keyAttributeOnNewNode.Value = keyAttribute.Value;
                }
            }
            return targetNode;
        }

        private XmlAttribute getKeyAttribute(XmlNode sourceNode)
        {
            XmlAttribute keyAttribute = null;
            for (int i = 0; i < sourceNode.Attributes.Count; i++)
            {
                if (sourceNode.Attributes[i].NamespaceURI == KeyedNodeIdentifyingNamespace)
                {
                    if (keyAttribute != null)
                    {
                        throw new MultipleKeyedAttributesException();
                    }
                    keyAttribute = sourceNode.Attributes[i];
                }
            }
            return keyAttribute;
        }

        private class MultipleKeyedAttributesException : Exception {}
    }
}
