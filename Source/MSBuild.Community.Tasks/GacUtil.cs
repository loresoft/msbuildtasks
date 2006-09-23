#region Copyright © 2006 Paul Welter. All rights reserved.
/*
Copyright © 2006 Paul Welter. All rights reserved.

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
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

// $Id$

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// The list of the commans available to the GacUtil Task
    /// </summary>
    public enum GacUtilCommands
    {
        /// <summary>Install the list of assemblies into the GAC.</summary>
        Install,
        /// <summary>Uninstall the list of assembly names from the GAC.</summary>
        Uninstall,
    }
    
    /// <summary>
    /// MSBuild task to install and uninstall asseblies into the GAC
    /// </summary>
    /// <example>Install a dll into the GAC.
    /// <code><![CDATA[
    ///     <GacUtil 
    ///         Command="Install" 
    ///         Assemblies="MSBuild.Community.Tasks.dll" 
    ///         Force="true" />
    /// ]]></code>
    /// </example>
    /// <example>Uninstall a dll from the GAC.
    /// <code><![CDATA[
    ///     <GacUtil 
    ///         Command="Uninstall" 
    ///         Assemblies="MSBuild.Community.Tasks" 
    ///         Force="true" />
    /// ]]></code>
    /// </example>
    public class GacUtil : ToolTask
    {

        private string _assemblyListFile;

        #region Properties
        private GacUtilCommands _command = GacUtilCommands.Install;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>The command.</value>
        /// <enum cref="MSBuild.Community.Tasks.GacUtilCommands"/>
        public string Command
        {
            get { return _command.ToString(); }
            set
            {
                if (Enum.IsDefined(typeof(GacUtilCommands), value))
                    _command = (GacUtilCommands)Enum.Parse(typeof(GacUtilCommands), value);
                else
                    throw new ArgumentException(
                        string.Format("The value '{0}' is not in the GacUtilCommans Enum.", value));
            }
        }

        private bool _force;

        /// <summary>
        /// Gets or sets a value indicating whether to force reinstall of an assembly.
        /// </summary>
        /// <value><c>true</c> if force; otherwise, <c>false</c>.</value>
        public bool Force
        {
            get { return _force; }
            set { _force = value; }
        }

        private string[] _assemblies;


        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
         [Required]
        public string[] Assemblies
        {
            get { return _assemblies; }
            set { _assemblies = value; }
        }

        #endregion

        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>
        /// The fully qualified path to the executable file.
        /// </returns>
        protected override string GenerateFullPathToTool()
        {
            return ToolLocationHelper.GetPathToDotNetFrameworkSdkFile(
                ToolName, TargetDotNetFrameworkVersion.Version20);
        }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the executable file to run.</returns>
        protected override string ToolName
        {
            get { return "gacutil.exe"; }
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
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>
        /// A string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {          
            CommandLineBuilder builder = new CommandLineBuilder();
            builder.AppendSwitch("/nologo");
            
            if (_force)
                builder.AppendSwitch("/f");

            switch(_command)
            {
                case GacUtilCommands.Install:
                    builder.AppendSwitch("/il");
                    break;
                case GacUtilCommands.Uninstall:
                    builder.AppendSwitch("/ul");
                    break;
            }
            builder.AppendFileNameIfNotNull(_assemblyListFile);
            return builder.ToString();
        }

        /// <summary>
        /// Runs the exectuable file with the specified task parameters.
        /// </summary>
        /// <returns>
        /// true if the task runs successfully; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            // write asseblies to text file so we can use /il and /ul switch
            _assemblyListFile = Path.GetTempFileName();
            File.WriteAllLines(_assemblyListFile, _assemblies);
            try
            {
                return base.Execute();
            }
            finally
            {
                File.Delete(_assemblyListFile);  // delete temp file
            }            
        }
        
    }
}
