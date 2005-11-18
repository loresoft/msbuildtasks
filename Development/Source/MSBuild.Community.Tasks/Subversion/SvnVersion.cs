// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;



namespace MSBuild.Community.Tasks.Subversion
{
    /// <summary>
    /// Summarize the local revision(s) of a working copy.
    /// </summary>
    /// <example>The following example gets the revision of the current folder.
    /// <code><![CDATA[
    /// <Target Name="Version">
    ///   <SvnVersion LocalPath=".">
    ///     <Output TaskParameter="Revision" PropertyName="Revision" />
    ///   </SvnVersion>
    ///   <Message Text="Revision: $(Revision)"/>
    /// </Target>
    /// ]]>
    /// </code>
    /// </example>
    public class SvnVersion : ToolTask
    {
        private static readonly Regex _numberRegex;
        private StringBuilder _outputBuffer;

        static SvnVersion()
        {
            _numberRegex = new Regex(@"\d+", RegexOptions.Compiled);
        }
        
        public SvnVersion()
        {
            _outputBuffer = new StringBuilder();
            base.ToolPath = GenerateFullPathToTool();           
        }

        #region Properties
        private string _localPath;

        /// <summary>Path to local working copy.</summary>
        [Required]
        public string LocalPath
        {
            get { return _localPath; }
            set { _localPath = value; }
        }

        /// <summary>Revision number of the local working repository.</summary>
        [Output]
        public int Revision
        {
            get { return _highRevision; }
            set { _highRevision = value; }
        }

        private int _highRevision = -1;

        /// <summary>High revision number of the local working repository revision range.</summary>
        [Output]
        public int HighRevision
        {
            get { return _highRevision; }
            set { _highRevision = value; }
        }

        private int _lowRevision = -1;

        /// <summary>Low revision number of the local working repository revision range.</summary>
        [Output]
        public int LowRevision
        {
            get { return _lowRevision; }
            set { _lowRevision = value; }
        }

        private bool _modifications = false;

        /// <summary>True if working copy contains modifications.</summary>
        [Output]
        public bool Modifications
        {
            get { return _modifications; }
            set { _modifications = value; }
        }

        private bool _switched = false;

        /// <summary>True if working copy is switched.</summary>
        [Output]
        public bool Switched
        {
            get { return _switched; }
            set { _switched = value; }
        }

        private bool _exported = false;

        /// <summary>
        /// True if invoked on a directory that is not a working copy, 
        /// svnversion assumes it is an exported working copy and prints "exported".
        /// </summary>
        [Output]
        public bool Exported
        {
            get { return _exported; }
            set { _exported = value; }
        }


        #endregion
        
        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                @"Subversion\bin"); ;
        }

        protected override MessageImportance StandardOutputLoggingImportance
        {
            get { return MessageImportance.Normal; }
        }

        protected override void LogToolCommand(string message)
        {
            Log.LogCommandLine(MessageImportance.Low, message);
        }

        protected override string ToolName
        {
            get { return "svnversion.exe"; }
        }

        protected override string GenerateCommandLineCommands()
        {
            DirectoryInfo localPath = new DirectoryInfo(_localPath);
            return string.Format("--no-newline {0}", localPath.FullName);
        }

        public override bool Execute()
        {
            bool result = base.Execute();
            if (result)
            {
                ParseOutput();
            }
            return result;
        }

        private void ParseOutput()
        {
            string buffer = _outputBuffer.ToString();
            MatchCollection revisions = _numberRegex.Matches(buffer);
            foreach (Match rm in revisions)
            {
                int revision;
                if (int.TryParse(rm.Value, out revision))
                {
                    _lowRevision = Math.Min(revision, _lowRevision);
                    _highRevision = Math.Max(revision, _highRevision);
                }
            }

            _modifications = buffer.Contains("M");
            _switched = buffer.Contains("S");
            _exported = buffer.Contains("exported");
            if (_exported) Log.LogWarning("LocalPath is not a working subversion copy.");

            Debug.WriteLine(string.Format("Revision: {0}; Modifications: {1}; Exported: {2};", 
                _highRevision, _modifications, _exported));
        }

        protected override void LogEventsFromTextOutput(string singleLine, Microsoft.Build.Framework.MessageImportance messageImportance)
        {
            base.LogEventsFromTextOutput(singleLine, messageImportance);
            _outputBuffer.Append(singleLine);
        }
        
    }
}
