// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Xml.XPath;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Updates a XML document using a XPath.
    /// </summary>
    /// <example>Update a XML element.
    /// <code><![CDATA[
    /// <XmlUpdate Prefix="n"
    ///     Namespace="http://schemas.microsoft.com/developer/msbuild/2003" 
    ///     XPath="/n:Project/n:PropertyGroup/n:TestUpdate"
    ///     XmlFileName="Subversion.proj"
    ///     Value="Test from $(MSBuildProjectFile)"/>
    /// ]]></code>
    /// </example>
    /// <remarks>
    /// The XML node being updated must exist before using the XmlUpdate task.
    /// </remarks>
    public class XmlUpdate : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:XmlUpdate"/> class.
        /// </summary>
        public XmlUpdate()
        {

        }

        #region Properties
        private string _xmlFileName;

        /// <summary>
        /// Gets or sets the name of the XML file.
        /// </summary>
        /// <value>The name of the XML file.</value>
        [Required]
        public string XmlFileName
        {
            get { return _xmlFileName; }
            set { _xmlFileName = value; }
        }

        private string _xpath;

        /// <summary>
        /// Gets or sets the XPath.
        /// </summary>
        /// <value>The XPath.</value>
        [Required]
        public string XPath
        {
            get { return _xpath; }
            set { _xpath = value; }
        }

        private string _value;

        /// <summary>
        /// Gets or sets the value to write.
        /// </summary>
        /// <value>The value.</value>
        [Required]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _namespace;

        /// <summary>
        /// Gets or sets the default namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        private string _prefix;

        /// <summary>
        /// Gets or sets the prefix to associate with the namespace being added.
        /// </summary>
        /// <value>The namespace prefix.</value>
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Updating Xml Document \"{0}\".", _xmlFileName);
                
                XmlDocument document = new XmlDocument();
                document.Load(_xmlFileName);
                
                XPathNavigator navigator = document.CreateNavigator();                
                XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);

                if (!string.IsNullOrEmpty(_prefix) && !string.IsNullOrEmpty(_namespace))
                {
                    manager.AddNamespace(_prefix, _namespace);
                }
                
                XPathExpression expression = XPathExpression.Compile(_xpath, manager);                
                XPathNodeIterator nodes = navigator.Select(expression);

                Log.LogMessage("  {0} node(s) selected for update.", nodes.Count);

                while (nodes.MoveNext())
                    nodes.Current.SetValue(_value);

                using (XmlTextWriter writer = new XmlTextWriter(_xmlFileName, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    document.Save(writer);
                    writer.Close();                    
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            Log.LogMessage("XmlUpdate Wrote: \"{0}\"", _value);
            return true;
        }
    }
}
