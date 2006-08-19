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
        /// 	/>]]></code>
        /// </example>
        public ITaskItem[] NamespaceDefinitions
        {
            get { return namespaceDefinitions; }
            set { namespaceDefinitions = value; }
        }
        private ITaskItem[] namespaceDefinitions;
        XmlNamespaceManager namespaceManager;


        /// <summary>
        /// The full path of the file containing content updated by the task
        /// </summary>
        [Output]
        public string ContentPathUsedByTask
        {
            get { return contentPathUsedByTask; }
        }
        private string contentPathUsedByTask;

        /// <summary>
        /// The full path of the file containing substitutions used by the task
        /// </summary>
        [Output]
        public string SubstitutionsPathUsedByTask
        {
            get { return substitutionsPathUsedByTask; }
        }
        string substitutionsPathUsedByTask;

        /// <summary>
        /// The full path of the file containing the results of the task
        /// </summary>
        [Output]
        public string MergedPathUsedByTask
        {
            get { return mergedPathUsedByTask; }
        }
        string mergedPathUsedByTask;

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

            setContentPath();

            setSubstitutionsPath();

            if (String.IsNullOrEmpty(substitutionsRoot)) substitutionsRoot = "/";
            if (String.IsNullOrEmpty(contentRoot)) contentRoot = "/";

            if (contentPathUsedByTask.Equals(substitutionsPathUsedByTask, StringComparison.InvariantCultureIgnoreCase) && (contentRoot == substitutionsRoot))
            {
                Log.LogError("The SubstitutionsRoot must be different from the ContentRoot when the ContentFile and SubstitutionsFile are the same.");
                return false;
            }

            XmlDocument contentDocument = LoadContentDocument();
            if (contentDocument == null) return false;

            XmlDocument substitutionsDocument = LoadSubstitutionsDocument();
            if (substitutionsDocument == null) return false;

            namespaceManager = new XmlNamespaceManager(contentDocument.NameTable);
            XmlTaskHelper.LoadNamespaceDefinitionItems(namespaceManager, namespaceDefinitions);

            XmlNode substitutionsRootNode = substitutionsDocument.SelectSingleNode(substitutionsRoot, namespaceManager);
            XmlNode configurationRootNode = contentDocument.SelectSingleNode(contentRoot, namespaceManager);

            try
            {
                addAllChildNodes(contentDocument, substitutionsRootNode, configurationRootNode);
            }
            catch (MultipleKeyedAttributesException)
            {
                Log.LogError("A substitution node contained more than one keyed attributed.");
                return false;
            }

            setMergedPath();
            return SaveMergedDocument(contentDocument);
        }

        /// <summary>
        /// Returns <see cref="SubstitutionsFile"/> as an <see cref="XmlDocument"/>.
        /// </summary>
        /// <remarks>This method is not intended for use by consumers. It is exposed for testing purposes.</remarks>
        /// <returns></returns>
        protected virtual XmlDocument LoadSubstitutionsDocument()
        {
            XmlDocument substitutionsDocument;
            if (contentPathUsedByTask.Equals(substitutionsPathUsedByTask, StringComparison.InvariantCultureIgnoreCase))
            {
                substitutionsDocument = LoadContentDocument();
            }
            else
            {
                if (!System.IO.File.Exists(substitutionsPathUsedByTask))
                {
                    Log.LogError("Unable to load substitutions file {0}", substitutionsPathUsedByTask);
                    return null;
                }
                substitutionsDocument = new XmlDocument();
                substitutionsDocument.Load(substitutionsPathUsedByTask);
            }
            return substitutionsDocument;
        }

        private void setSubstitutionsPath()
        {
            substitutionsPathUsedByTask = substitutionsFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(substitutionsPathUsedByTask)) substitutionsPathUsedByTask = substitutionsFile.ItemSpec;
        }

        private void setContentPath()
        {
            contentPathUsedByTask = contentFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(contentPathUsedByTask)) contentPathUsedByTask = contentFile.ItemSpec;
        }

        /// <summary>
        /// Returns <see cref="ContentFile"/> as an <see cref="XmlDocument"/>.
        /// </summary>
        /// <remarks>This method is not intended for use by consumers. It is exposed for testing purposes.</remarks>
        /// <returns></returns>
        protected virtual XmlDocument LoadContentDocument()
        {
            if (!System.IO.File.Exists(contentPathUsedByTask))
            {
                Log.LogError("Unable to load content file {0}", contentPathUsedByTask);
                return null;
            }
            XmlDocument contentDocument = new XmlDocument();
            contentDocument.Load(contentPathUsedByTask);
            return contentDocument;
        }

        /// <summary>
        /// Creates <see cref="MergedFile"/> from the specified <see cref="XmlDocument"/>
        /// </summary>
        /// <param name="mergedDocument">The XML to save to a file</param>
        /// <remarks>This method is not intended for use by consumers. It is exposed for testing purposes.</remarks>
        /// <returns></returns>
        protected virtual bool SaveMergedDocument(XmlDocument mergedDocument)
        {
            try
            {
                mergedDocument.Save(mergedPathUsedByTask);
            }
            catch (System.IO.IOException exception)
            {
                Log.LogError("Unable to create MergedFile - {0}", exception.Message);
                return false;
            }
            return true;
        }

        private void setMergedPath()
        {
            mergedPathUsedByTask = mergedFile.GetMetadata("FullPath");
            if (String.IsNullOrEmpty(mergedPathUsedByTask)) mergedPathUsedByTask = mergedFile.ItemSpec;
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
                        if (sourceAttribute.NamespaceURI != keyedNodeIdentifyingNamespace)
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
                Log.LogMessage(MessageImportance.Low, "Creating attribute '{0}' on '{1}'", attributeName, getFullPathOfNode(modifiedNode));
                targetAttribute = modifiedNode.Attributes.Append(config.CreateAttribute(attributeName));
            }
            targetAttribute.Value = attributeValue;
            Log.LogMessage("Setting attribute '{0}' to '{1}' on '{2}'", targetAttribute.Name, targetAttribute.Value, getFullPathOfNode(modifiedNode));
        }

        private string getFullPathOfNode(XmlNode node)
        {
            string fullPath = String.Empty;
            XmlNode currentNode = node;
            while (currentNode != null && currentNode.NodeType != XmlNodeType.Document)
            {
                fullPath = "/" + currentNode.Name + fullPath;
                currentNode = currentNode.ParentNode;
            }
            return fullPath;
        }

        /// <summary>
        /// The namespace used to decorate attributes that should be used as a key to locate an existing node.
        /// </summary>
        /// <remarks>Evaluates to: <c>urn:msbuildcommunitytasks-xmlmassupdate</c></remarks>
        public string KeyedNodeIdentifyingNamespace { get { return keyedNodeIdentifyingNamespace; } }
        private const string keyedNodeIdentifyingNamespace = "urn:msbuildcommunitytasks-xmlmassupdate";

        private XmlNode ensureNode(XmlDocument config, XmlNode destinationParentNode, XmlNode nodeToEnsure)
        {
            XmlAttribute keyAttribute = getKeyAttribute(nodeToEnsure);
            string xpath;
            if (keyAttribute == null)
            {
                xpath = nodeToEnsure.Name;
            }
            else
            {
                Log.LogMessage(MessageImportance.Low, "Using keyed attribute '{0}={1}' to locate node '{2}'", keyAttribute.LocalName,keyAttribute.Value, getFullPathOfNode(destinationParentNode) + "/" + nodeToEnsure.LocalName);
                xpath = String.Format("{0}[@{1}='{2}']", nodeToEnsure.LocalName, keyAttribute.LocalName, keyAttribute.Value);
            }
            XmlNode ensuredNode = destinationParentNode.SelectSingleNode(xpath, namespaceManager);
            if (ensuredNode == null)
            {
                ensuredNode = destinationParentNode.AppendChild(config.CreateNode(XmlNodeType.Element, nodeToEnsure.Name, String.Empty));
                Log.LogMessage(MessageImportance.Low, "Created node '{0}'", getFullPathOfNode(ensuredNode));
                if (keyAttribute != null)
                {
                    XmlAttribute keyAttributeOnNewNode = ensuredNode.Attributes.Append(config.CreateAttribute(keyAttribute.LocalName));
                    keyAttributeOnNewNode.Value = keyAttribute.Value;
                }
            }
            return ensuredNode;
        }

        private XmlAttribute getKeyAttribute(XmlNode sourceNode)
        {
            XmlAttribute keyAttribute = null;
            for (int i = 0; i < sourceNode.Attributes.Count; i++)
            {
                if (sourceNode.Attributes[i].NamespaceURI == keyedNodeIdentifyingNamespace)
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
