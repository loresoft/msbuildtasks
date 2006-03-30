using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;

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
        directory,
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
    /// This example will determine the Subversion repository root for the.
    /// current directory and print it out.
    /// <code><![CDATA[
    /// <Target Name="printinfo">
    ///   <SvnInfo LocalPath=".">
    ///     <Output TaskParameter="RepositoryRoot" PropertyName="root" />
    ///   </SvnInfo>
    ///   <Message Text="root: $(root)" />
    /// </Target>
    /// ]]></code>
    /// </example>
    public class SvnInfo : SvnClient
    {
        private string m_strRepositoryRoot;
        private Guid m_Guid;
        private NodeKind m_eNodeKind = Subversion.NodeKind.unknown;
        private string m_strLastChangedAuthor;
        private int m_nLastChangedRev;
        private System.DateTime m_LastChangedDate;
        private Schedule m_eSchedule;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnInfo"/> class.
        /// </summary>
        public SvnInfo()
        {
            base.Command = "info";
            ResetMemberVariables();
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
        /// Indicates whether all task paratmeters are valid.
        /// </summary>
        /// <returns>
        /// true if all task parameters are valid; otherwise, false.
        /// </returns>
        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(base.LocalPath))
            {
                Log.LogError(Properties.Resources.ParameterRequired, "SvnInfo", "LocalPath");
                return false;
            }
            return base.ValidateParameters();
        }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <returns>true if execution is successful, false if not.</returns>
        public override bool Execute()
        {
            bool bSuccess = base.Execute();

            if (!bSuccess)
            {
                ResetMemberVariables();
            }

            return bSuccess;
        }

        /// <summary>
        /// "svn.exe info" prints out key/value pairs separated by a colon. 
        /// </summary>
        /// <param name="strLine">A line of text printed out by svn.exe</param>
        /// <param name="strKey">The key string. empty if no key/value found.</param>
        /// <param name="strValue">The value string. empty of no key/value found.</param>
        /// <returns>true if a key/value pair found. false if not</returns>
        private bool ExtractKeyValuePair(string strLine, out string strKey, out string strValue)
        {
            // Here is a sample output:
            //
            //Path: .
            //URL: svn://cmumford/CoreMSBuildTasks
            //Repository Root: svn://cmumford
            //Repository UUID: 48e252eb-c463-fd45-8db3-81f6beb5f896
            //Revision: 225
            //Node Kind: directory
            //Schedule: normal
            //Last Changed Author: cmumford
            //Last Changed Rev: 223
            //Last Changed Date: 2006-02-09 14:36:05 -0600 (Thu, 09 Feb 2006)

            int nColonIdx = strLine.IndexOf(':');
            if (nColonIdx == -1)
            {
                strKey = string.Empty;
                strValue = string.Empty;
                return false;
            }

            strKey = strLine.Substring(0, nColonIdx);
            strValue = strLine.Substring(nColonIdx + 2);

            return true;
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
        /// Parse a subversion date/time value. They print out a date that
        /// looks like this "2006-02-09 14:36:05 -0600 (Thu, 09 Feb 2006)" which
        /// isn't directly parsable by the DateTime class.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        private System.DateTime ParseSvnDate(string datetime)
        {
            int idx = datetime.IndexOf('(');
            if (idx == -1)
            {
                return System.DateTime.Parse(datetime);
            }
            else
            {
                return System.DateTime.Parse(datetime.Substring(0, idx));
            }
        }

        /// <summary>
        /// Parse the text output from the command and log the lines.
        /// </summary>
        /// <param name="singleLine">One line of text output from the tool being run.</param>
        /// <param name="messageImportance">The message importance.</param>
        protected override void LogEventsFromTextOutput(string singleLine, Microsoft.Build.Framework.MessageImportance messageImportance)
        {
            // The base implementation just seems to log out the lines which is really chatty.
            // base.LogEventsFromTextOutput(singleLine, messageImportance);

            string key;
            string value;

            if (ExtractKeyValuePair(singleLine, out key, out value))
            {
                if (key == "URL")
                {
                    RepositoryPath = value;
                }
                else if (key == "Repository Root")
                {
                    m_strRepositoryRoot = value;
                }
                else if (key == "Repository UUID")
                {
                    m_Guid = new Guid(value);
                }
                else if (key == "Revision")
                {
                    Revision = int.Parse(value);
                }
                else if (key == "Last Changed Rev")
                {
                    m_nLastChangedRev = int.Parse(value);
                }
                else if (key == "Last Changed Author")
                {
                    m_strLastChangedAuthor = value;
                }
                else if (key == "Last Changed Date")
                {
                    m_LastChangedDate = ParseSvnDate(value);
                }
                else if (key == "Schedule")
                {
                    if (value == Subversion.Schedule.normal.ToString())
                    {
                        m_eSchedule = Subversion.Schedule.normal;
                    }
                    else
                    {
                        Log.LogError("Unknown schedule: " + value);
                    }
                }
                else if (key == "Node Kind")
                {
                    if (value == Subversion.NodeKind.directory.ToString())
                    {
                        m_eNodeKind = Subversion.NodeKind.directory;
                    }
                    else if (value == Subversion.NodeKind.file.ToString())
                    {
                        m_eNodeKind = Subversion.NodeKind.file;
                    }
                    else
                    {
                        Log.LogError("Unknown node kind: " + value);
                    }
                }
            }
        }
    }
}
