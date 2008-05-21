#region Copyright © 2005 Paul Welter. All rights reserved.
/*
Copyright © 2005 Paul Welter. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Win32;

// $Id$

namespace MSBuild.Community.Tasks.Subversion
{

	/// <summary>
	/// Subversion client base class
	/// </summary>
	public class SvnClient : ToolTask
	{
		#region Fields
		private const string _switchBooleanFormat = " --{0}";
		private const string _switchStringFormat = " --{0} \"{1}\"";
		private const string _switchValueFormat = " --{0} {1}";

		private static readonly Regex _revisionParse = new Regex(@"\b(?<Rev>\d+)", RegexOptions.Compiled);

		#endregion Fields

		#region Enums
		[Flags]
		internal enum SvnSwitches
		{
            None = 0,
            NonInteractive = 0x01,
            NoAuthCache = 0x02,
            Xml = 0x04
		}

		#endregion Enums

		#region Input Parameters
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
		[Output]
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

		#endregion Input Parameters

		#region Output Parameters
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

		#endregion Output Parameters

		#region Internal Properties
		private SvnSwitches _commandSwitches = SvnSwitches.None;

		/// <summary>
		/// Gets or sets the command switchs.
		/// </summary>
		/// <value>The command switchs.</value>
		internal SvnSwitches CommandSwitches
		{
			get { return _commandSwitches; }
			set { _commandSwitches = value; }
		}

		#endregion Internal Properties

		#region Protected Methods
		/// <summary>
		/// Generates the SVN command.
		/// </summary>
		/// <returns></returns>
		protected virtual string GenerateSvnCommand()
		{
			StringBuilder builder = new StringBuilder();


			builder.Append(_command);

			if (!string.IsNullOrEmpty(_repositoryPath))
				builder.AppendFormat(" \"{0}\"", _repositoryPath);

			if (!string.IsNullOrEmpty(_localPath))
				builder.AppendFormat(" \"{0}\"", _localPath);

			if (_revision >=0)
				builder.AppendFormat(_switchValueFormat, "revision", _revision);

			if (_targets != null)
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

            if ((CommandSwitches & SvnSwitches.Xml) == SvnSwitches.Xml)
            {
                builder.AppendFormat(_switchBooleanFormat, "xml");
            }

            if ((CommandSwitches & SvnSwitches.NonInteractive) == SvnSwitches.NonInteractive)
            {
                builder.AppendFormat(_switchBooleanFormat, "non-interactive");
            }
            if ((CommandSwitches & SvnSwitches.NoAuthCache) == SvnSwitches.NoAuthCache)
            {
                builder.AppendFormat(_switchBooleanFormat, "no-auth-cache");
            }
            
            return builder.ToString();
		}

		#endregion Protected Methods

		#region Task Overrides
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
            string path = SvnClient.FindToolPath(ToolName);
            base.ToolPath = path;

            return Path.Combine(ToolPath, ToolName);
        }

        public static string FindToolPath(string toolName)
        {
            string toolPath = null;
            // 1) check registry
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\" + toolName, false);
            if (key != null)
            {
                string possiblePath = key.GetValue(null) as string;
                if (File.Exists(possiblePath))
                {
                    toolPath = Path.GetDirectoryName(possiblePath);
                }
            }
            // 2) search the path
            if (toolPath == null)
            {
                string pathEnvironmentVariable = Environment.GetEnvironmentVariable("PATH");
                string[] paths = pathEnvironmentVariable.Split(Path.PathSeparator);
                foreach (string path in paths)
                {
                    string fullPathToClient = Path.Combine(path, toolName);
                    if (File.Exists(fullPathToClient))
                    {
                        toolPath = path;
                        break;
                    }
                }
            }
            // 3) try default install location
            if (toolPath == null)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Subversion\bin");
                string fullPathToClient = Path.Combine(path, toolName);
                if (File.Exists(fullPathToClient))
                {
                    toolPath = path;
                }
            }
            // 4) try default CollabNet install location
            if (toolPath == null)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"CollabNet Subversion Server");
                string fullPathToClient = Path.Combine(path, toolName);
                if (File.Exists(fullPathToClient))
                {
                    toolPath = path;
                }
            }

            return toolPath;
        }

        /// <summary>
        /// Logs the starting point of the run to all registered loggers.
        /// </summary>
        /// <param name="message">A descriptive message to provide loggers, usually the command line and switches.</param>
        protected override void LogToolCommand(string message)
        {
            Log.LogCommandLine(MessageImportance.Low, message);
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
            get
            {
                return "svn.exe";
            }
		}

		#endregion Task Overrides

	}
}
