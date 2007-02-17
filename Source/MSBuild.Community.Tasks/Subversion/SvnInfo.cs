using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using System.Xml;

// $Id: SvnCheckout.cs 102 2006-01-09 18:01:13Z iko $

namespace MSBuild.Community.Tasks.Subversion
{
    /// <summary>
    /// The kind of Subversion node. The names match the text output
    /// by "svn info".
    /// </summary>
    public enum NodeKind
    {
        /// <summary>
        /// Node is a file
        /// </summary>
        file,
        /// <summary>
        /// Node is a directory
        /// </summary>
        dir,
        /// <summary>
        /// Unknown node type
        /// </summary>
        unknown
    }

    /// <summary>
    /// The Subversion schedule type.
    /// </summary>
    public enum Schedule
    {
        /// <summary>
        /// Normal schedule
        /// </summary>
        normal,
        /// <summary>
        /// Unknown schedule.
        /// </summary>
        unknown
    }

    /// <summary>
    /// Run the "svn info" command and parse the output
    /// </summary>
    /// <example>
    /// This example will determine the Subversion repository root for
    /// a working directory and print it out.
    /// <code><![CDATA[
    /// <Target Name="printinfo">
    ///   <SvnInfo LocalPath="c:\code\myapp">
    ///     <Output TaskParameter="RepositoryRoot" PropertyName="root" />
    ///   </SvnInfo>
    ///   <Message Text="root: $(root)" />
    /// </Target>
    /// ]]></code>
    /// </example>
    /// <remarks>You can retrieve Subversion information for a <see cref="SvnClient.LocalPath"/> or <see cref="SvnClient.RepositoryPath"/>.
    /// If you do not provide a value for <see cref="SvnClient.LocalPath"/> or <see cref="SvnClient.RepositoryPath"/>, the current directory is assumed.</remarks>
    public class SvnInfo : SvnClient
    {
        private string m_strRepositoryRoot;
        private Guid m_Guid;
        private NodeKind m_eNodeKind = Subversion.NodeKind.unknown;
        private string m_strLastChangedAuthor;
        private int m_nLastChangedRev;
        private System.DateTime m_LastChangedDate;
        private Schedule m_eSchedule;
        private StringBuilder _outputBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnInfo"/> class.
        /// </summary>
        public SvnInfo()
        {
            base.Command = "info";
            base.CommandSwitches = SvnSwitches.NonInteractive | SvnSwitches.NoAuthCache | SvnSwitches.Xml;
            ResetMemberVariables();
            _outputBuffer = new StringBuilder();
        }

        /// <summary>
        /// Reset all instance variables to their default (unset) state.
        /// </summary>
        private void ResetMemberVariables()
        {
            RepositoryPath = string.Empty;
            m_strRepositoryRoot = string.Empty;
            m_Guid = Guid.Empty;
            m_eNodeKind = Subversion.NodeKind.unknown;
            m_strLastChangedAuthor = string.Empty;
            m_nLastChangedRev = 0;
            m_LastChangedDate = DateTime.Now;
            m_eSchedule = Subversion.Schedule.unknown;
        }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <returns>true if execution is successful, false if not.</returns>
        public override bool Execute()
        {
            bool bSuccess = base.Execute();
            
            if (bSuccess)
            {
                ParseOutput();
            }
            else
            {
                ResetMemberVariables();
            }

            return bSuccess;
        }

        private void ParseOutput()
        {
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(_outputBuffer.ToString());
            XmlNode entryNode = xmlDoc.SelectSingleNode("/info/entry");
            if (entryNode == null) return;
            string foundValue;

            foundValue = getAttributeValue(entryNode, "revision");
            if (!String.IsNullOrEmpty(foundValue))
            {
                Revision = Int32.Parse(foundValue);
            }

            foundValue = getAttributeValue(entryNode, "kind");
            if (!String.IsNullOrEmpty(foundValue))
            {
                if (foundValue == Subversion.NodeKind.dir.ToString())
                {
                    m_eNodeKind = Subversion.NodeKind.dir;
                }
                else if (foundValue == Subversion.NodeKind.file.ToString())
                {
                    m_eNodeKind = Subversion.NodeKind.file;
                }
                else
                {
                    Log.LogError("Unknown node kind: " + foundValue);
                }
            }
            string nodeText;

            nodeText = getNodeText(entryNode, "url");
            if (!String.IsNullOrEmpty(nodeText))
            {
                RepositoryPath = nodeText;
            }

            nodeText = getNodeText(entryNode, "repository/root");
            if (!String.IsNullOrEmpty(nodeText))
            {
                m_strRepositoryRoot = nodeText;
            }

            nodeText = getNodeText(entryNode, "repository/uuid");
            if (!String.IsNullOrEmpty(nodeText))
            {
                m_Guid = new Guid(nodeText);
            }

            XmlNode childNode;
            childNode = entryNode.SelectSingleNode("commit");
            if (childNode != null)
            {
                foundValue = getAttributeValue(childNode, "revision");
                if (!String.IsNullOrEmpty(foundValue))
                {
                    m_nLastChangedRev = Int32.Parse(foundValue);
                }

                nodeText = getNodeText(childNode, "author");
                if (!String.IsNullOrEmpty(nodeText))
                {
                    m_strLastChangedAuthor = nodeText;
                }

                nodeText = getNodeText(childNode, "date");
                if (!String.IsNullOrEmpty(nodeText))
                {
                    m_LastChangedDate = DateTime.Parse(nodeText);
                }
            }

            nodeText = getNodeText(entryNode, "wc-info/schedule");
            if (!String.IsNullOrEmpty(nodeText))
            {
                if (nodeText == Subversion.Schedule.normal.ToString())
                {
                    m_eSchedule = Subversion.Schedule.normal;
                }
                else
                {
                    Log.LogError("Unknown schedule: " + nodeText);
                }
            }
        }

        private string getAttributeValue(XmlNode node, string attributeName)
        {
            XmlAttribute attribute = node.Attributes[attributeName];
            if (attribute == null) return null;
            return attribute.Value;
        }

        private string getNodeText(XmlNode parentNode, string xpath)
        {
            XmlNode node = parentNode.SelectSingleNode(xpath);
            if (node == null) return null;
            return node.InnerText;
        }

        /// <summary>
        /// Return the repository root or null if not set by Subversion.
        /// </summary>
        [Output]
        public string RepositoryRoot
        {
            get
            {
                return m_strRepositoryRoot;
            }
        }

        /// <summary>
        /// Return the repository UUID value from Subversion.
        /// </summary>
        [Output]
        public string RepositoryUuid
        {
            get
            {
                return m_Guid.ToString();
            }
        }

        /// <summary>
        /// The Subversion node kind.
        /// </summary>
        /// <enum cref="MSBuild.Community.Tasks.Subversion.NodeKind"/>
        [Output]
        public string NodeKind
        {
            get
            {
                return m_eNodeKind.ToString();
            }
        }

        /// <summary>
        /// The author who last changed this node.
        /// </summary>
        [Output]
        public string LastChangedAuthor
        {
            get
            {
                return m_strLastChangedAuthor;
            }
        }

        /// <summary>
        /// The last changed revision number.
        /// </summary>
        [Output]
        public int LastChangedRevision
        {
            get
            {
                return m_nLastChangedRev;
            }
        }

        /// <summary>
        /// The date this node was last changed.
        /// </summary>
        [Output]
        public string LastChangedDate
        {
            get
            {
                return m_LastChangedDate.ToString();
            }
        }

        /// <summary>
        /// The Subversion schedule type.
        /// </summary>
        /// <enum cref="MSBuild.Community.Tasks.Subversion.Schedule"/>
        [Output]
        public string Schedule
        {
            get
            {
                return m_eSchedule.ToString();
            }
        }

		/// <summary>
		/// Logs the events from text output.
		/// </summary>
		/// <param name="singleLine">The single line.</param>
		/// <param name="messageImportance">The message importance.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            if (messageImportance == MessageImportance.High)
            {
                base.LogEventsFromTextOutput(singleLine, messageImportance);
            }
            _outputBuffer.Append(singleLine);
        }
        
    }
}
