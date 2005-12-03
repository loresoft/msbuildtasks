// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace MSBuild.Community.Tasks.Subversion
{

    /// <summary>
    /// Subversion client base class
    /// </summary>
    public class SvnClient : ToolTask
    {
        private const string _switchBooleanFormat = " --{0}";
        private const string _switchStringFormat = " --{0} \"{1}\"";
        private const string _switchValueFormat = " --{0} {1}";

        private static readonly Regex _revisionParse = new Regex(@"revision (?<Rev>\d+)", RegexOptions.Compiled);

        [Flags]
        internal enum SvnSwitches
        {
            RepositoryPath,
            LocalPath,
            Targets,
            Revision,
            Default = SvnSwitches.RepositoryPath | SvnSwitches.LocalPath | SvnSwitches.Revision,
            All = SvnSwitches.Default | SvnSwitches.Targets 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SvnClient"/> class.
        /// </summary>
        public SvnClient()
        {

        }

        #region Properties
        private string _command;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private string _arguments;

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public string Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        private string _username;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _password;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private bool? _verbose;

        /// <summary>
        /// Gets or sets the verbose.
        /// </summary>
        /// <value>The verbose.</value>
        public bool? Verbose
        {
            get { return _verbose; }
            set { _verbose = value; }
        }

        private bool? _force;

        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        /// <value>The force.</value>
        public bool? Force
        {
            get { return _force; }
            set { _force = value; }
        }

        private string _message;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _repositoryPath;

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set { _repositoryPath = value; }
        }

        private string _localPath;

        /// <summary>
        /// Gets or sets the local path.
        /// </summary>
        /// <value>The local path.</value>
        public string LocalPath
        {
            get { return _localPath; }
            set { _localPath = value; }
        }

        private int _revision = -1;

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>The revision.</value>
        [Output]
        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        private ITaskItem[] _targets;

        /// <summary>
        /// Gets or sets the targets.
        /// </summary>
        /// <value>The targets.</value>
        public ITaskItem[] Targets
        {
            get { return _targets; }
            set { _targets = value; }
        }
        
        private SvnSwitches _commandSwitchs = SvnSwitches.Default;

        /// <summary>
        /// Gets or sets the command switchs.
        /// </summary>
        /// <value>The command switchs.</value>
        internal SvnSwitches CommandSwitchs
        {
            get { return _commandSwitchs; }
            set { _commandSwitchs = value; }
        }
        
        #endregion

        /// <summary>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>
        /// A string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            return GenerateSvnCommand() + GenerateSvnArguments();
        }

        /// <summary>
        /// Generates the SVN command.
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateSvnCommand()
        {
            StringBuilder builder = new StringBuilder();

            
            builder.Append(_command);
            if (!string.IsNullOrEmpty(_repositoryPath) && (CommandSwitchs & SvnSwitches.RepositoryPath) == SvnSwitches.RepositoryPath)
                builder.AppendFormat(" \"{0}\"", _repositoryPath);

            if (!string.IsNullOrEmpty(_localPath) && (CommandSwitchs & SvnSwitches.LocalPath) == SvnSwitches.LocalPath)
                builder.AppendFormat(" \"{0}\"", _localPath);

            if (_revision > 0 && (CommandSwitchs & SvnSwitches.Revision) == SvnSwitches.Revision)
                builder.AppendFormat(_switchValueFormat, "revision", _revision);

            if (_targets != null && (CommandSwitchs & SvnSwitches.Targets) == SvnSwitches.Targets)
            {
                foreach (ITaskItem fileTarget in _targets)
                {
                    builder.AppendFormat(" \"{0}\"", fileTarget.ItemSpec);
                }                
            }

            return builder.ToString();
        }



        /// <summary>
        /// Generates the SVN arguments.
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateSvnArguments()
        {

            StringBuilder builder = new StringBuilder();
            
            if (!string.IsNullOrEmpty(_username))
                builder.AppendFormat(_switchValueFormat, "username", _username);
            
            if (!string.IsNullOrEmpty(_password))
                builder.AppendFormat(_switchValueFormat, "password", _password);

            if (!string.IsNullOrEmpty(_message))
                builder.AppendFormat(_switchStringFormat, "message", _message);

            if (_force.HasValue && _force.Value)
                builder.AppendFormat(_switchBooleanFormat, "force");

            if (_verbose.HasValue && _verbose.Value)
                builder.AppendFormat(_switchBooleanFormat, "verbose");

            if (!string.IsNullOrEmpty(_arguments))
                builder.AppendFormat(" {0}", _arguments);

            // all commands should be non interactive
            builder.AppendFormat(_switchBooleanFormat, "non-interactive");
            builder.AppendFormat(_switchBooleanFormat, "no-auth-cache"); 
            
            return builder.ToString();           
        }

        /// <summary>
        /// Indicates whether all task paratmeters are valid.
        /// </summary>
        /// <returns>
        /// true if all task parameters are valid; otherwise, false.
        /// </returns>
        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(_command))
            {
                Log.LogError(Properties.Resources.ParameterRequired, "SvnClient", "Command");
                return false;
            }
            return base.ValidateParameters();
        }

        /// <summary>
        /// Logs the events from text output.
        /// </summary>
        /// <param name="singleLine">The single line.</param>
        /// <param name="messageImportance">The message importance.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            base.LogEventsFromTextOutput(singleLine, messageImportance);

            Match revMatch = _revisionParse.Match(singleLine);
            if (revMatch.Success)
            {
                string tempRev = revMatch.Groups["Rev"].Value;
                int.TryParse(tempRev, out _revision);
            }
        }

        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>
        /// The fully qualified path to the executable file.
        /// </returns>
        protected override string GenerateFullPathToTool()
        {
            base.ToolPath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
               @"Subversion\bin");
            return  Path.Combine(ToolPath, ToolName);
        }

        /// <summary>
        /// Gets the <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.</returns>
        protected override MessageImportance StandardOutputLoggingImportance
        {
            get { return MessageImportance.Normal; }
        }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the executable file to run.</returns>
        protected override string ToolName
        {
            get { return "svn.exe"; }
        }
    }
}
