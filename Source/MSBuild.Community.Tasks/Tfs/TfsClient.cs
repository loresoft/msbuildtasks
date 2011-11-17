#region Copyright © 2011 Paul Welter. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

// $Id$

namespace MSBuild.Community.Tasks.Tfs
{
    public class TfsClient : ToolTask
    {

        public string Command { get; set; }

        public ITaskItem[] Files { get; set; }

        public bool Recursive { get; set; }
        public bool All { get; set; }
        public bool Overwrite { get; set; }
        public bool Override { get; set; }
        public bool Force { get; set; }
        public bool Preview { get; set; }
        public bool Remap { get; set; }
        public bool Silent { get; set; }
        public bool Saved { get; set; }
        public bool Validate { get; set; }
        public bool Bypass { get; set; }

        public string Comment { get; set; }
        public string Version { get; set; }
        public string Lock { get; set; }
        public string Type { get; set; }
        public string Author { get; set; }
        public string Notes { get; set; }
        public string Format { get; set; }
        public string Collection { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string WorkspaceName { get; set; }
        public string WorkspaceOwner { get; set; }
        public string ShelveSetName { get; set; }
        public string ShelveSetOwner { get; set; }

        
        [Output]
        public string Changeset { get; set; }
        [Output]
        public string ServerPath { get; set; }
        

        private string FindToolPath(string toolName)
        {
            return string.Empty;
        }

        protected virtual void GenerateCommand(CommandLineBuilder builder)
        {
            builder.AppendSwitch(Command);
            builder.AppendFileNamesIfNotNull(Files, " ");
        }

        protected virtual void GenerateArguments(CommandLineBuilder builder)
        {
            builder.AppendSwitch("/noprompt");

            builder.AppendSwitchIfNotNull("/comment:", Comment);
            builder.AppendSwitchIfNotNull("/version:", Version);
            builder.AppendSwitchIfNotNull("/lock:", Lock);
            builder.AppendSwitchIfNotNull("/type:", Type);
            builder.AppendSwitchIfNotNull("/author:", Author);
            builder.AppendSwitchIfNotNull("/notes:", Notes);
            builder.AppendSwitchIfNotNull("/format:", Format);
            builder.AppendSwitchIfNotNull("/collection:", Collection);

            if (Recursive)
                builder.AppendSwitch("/recursive");
            if (All)
                builder.AppendSwitch("/all");
            if (Override)
                builder.AppendSwitch("/override");
            if (Overwrite)
                builder.AppendSwitch("/overwrite");
            if (Force)
                builder.AppendSwitch("/force");
            if (Preview)
                builder.AppendSwitch("/preview");
            if (Remap)
                builder.AppendSwitch("/remap");
            if (Silent)
                builder.AppendSwitch("/silent");
            if (Saved)
                builder.AppendSwitch("/saved");
            if (Validate)
                builder.AppendSwitch("/validate");
            if (Bypass)
                builder.AppendSwitch("/bypass");

            if (!string.IsNullOrEmpty(UserName))
            {
                string login = "/login:" + UserName;
                if (!string.IsNullOrEmpty(Password))
                    login += "," + Password;

                builder.AppendSwitch(login);
            }
            
            if (!string.IsNullOrEmpty(WorkspaceName))
            {
                string workspace = "/workspace:" + WorkspaceName;
                if (!string.IsNullOrEmpty(WorkspaceOwner))
                    workspace += "," + WorkspaceOwner;

                builder.AppendSwitch(workspace);
            }

            if (!string.IsNullOrEmpty(ShelveSetName))
            {
                string shelveset = "/shelveset:" + ShelveSetName;
                if (!string.IsNullOrEmpty(ShelveSetOwner))
                    shelveset += "," + ShelveSetOwner;

                builder.AppendSwitch(shelveset);
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
            if (string.IsNullOrEmpty(ToolPath))
                ToolPath = FindToolPath(ToolName);

            return Path.Combine(ToolPath, ToolName);
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
        /// <returns>
        /// The name of the executable file to run.
        /// </returns>
        protected override string ToolName
        {
            get { return "tf.exe"; }
        }

        /// <summary>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>
        /// A string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            var commandLine = new CommandLineBuilder();
            GenerateCommand(commandLine);
            GenerateArguments(commandLine);

            return commandLine.ToString();
        }
      
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
          bool isError = messageImportance == StandardErrorLoggingImportance;

          if (isError)
            base.LogEventsFromTextOutput(singleLine, messageImportance);

          Match m = Regex.Match(singleLine, @"(?<Name>[\w ]+)\s*\:(?<Value>[^\r\n]+)");
          if (!m.Success)
            return;

          string name = m.Groups["Name"].Value.Trim();
          string value = m.Groups["Value"].Value.Trim();

          switch (name)
          {
            case "Changeset":
              Changeset = value;
              break;
            case "Server path":
              ServerPath = value;
              break;
          }
        }

        /// <summary>
        /// Indicates whether all task paratmeters are valid.
        /// </summary>
        /// <returns>
        /// true if all task parameters are valid; otherwise, false.
        /// </returns>
        protected override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(Command))
            {
                Log.LogError(Properties.Resources.ParameterRequired, "TfsClient", "Command");
                return false;
            }
            return base.ValidateParameters();
        }
    }
}
