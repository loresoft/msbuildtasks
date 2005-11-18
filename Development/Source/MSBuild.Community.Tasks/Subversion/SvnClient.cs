// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;

namespace MSBuild.Community.Tasks.Subversion
{

    public class SvnClient : ToolTask
    {
        private const string _switchBooleanFormat = " --{0}";
        private const string _switchStringFormat = " --{0} \"{1}\"";
        private const string _switchValueFormat = " --{0} {1}";

        [Flags]
        internal enum SvnSwitches
        {
            RepositoryPath,
            LocalPath,
            Files,
            Revision,
            Default = SvnSwitches.RepositoryPath | SvnSwitches.LocalPath | SvnSwitches.Revision,
            All = SvnSwitches.Default | SvnSwitches.Files 
        }

        public SvnClient()
        {
            base.ToolPath = GenerateFullPathToTool();
        }

        #region Properties
        private string _command;

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private string _arguments;

        public string Arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private bool? _verbose;

        public bool? Verbose
        {
            get { return _verbose; }
            set { _verbose = value; }
        }

        private bool? _force;

        public bool? Force
        {
            get { return _force; }
            set { _force = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        private string _repositoryPath;

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set { _repositoryPath = value; }
        }

        private string _localPath;

        public string LocalPath
        {
            get { return _localPath; }
            set { _localPath = value; }
        }

        private int _revision = -1;

        [Output]
        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }


        private SvnSwitches _commandSwitchs = SvnSwitches.Default;

        internal SvnSwitches CommandSwitchs
        {
            get { return _commandSwitchs; }
            set { _commandSwitchs = value; }
        }
        
        #endregion
       
        protected override string GenerateCommandLineCommands()
        {
            return GenerateSvnCommand() + GenerateSvnArguments();
        }

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

            return builder.ToString();
        }

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

            builder.Append(" --non-interactive --no-auth-cache"); // all commands should be non interactive

            return builder.ToString();
           
        }

        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(_command))
            {
                Log.LogError(MSBuild.Community.Tasks.Properties.Resources.ParameterRequired, "SvnClient", "Command");
                return false;
            }
            return base.ValidateParameters();
        }

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
        
        protected override string ToolName
        {
            get { return "svn.exe"; }
        }
    }
}
