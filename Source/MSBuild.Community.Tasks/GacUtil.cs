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
    public enum GacUtilCommands
    {
        Install,
        Uninstall,
    }
    
    public class GacUtil : ToolTask
    {


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
            set { _command = (GacUtilCommands)Enum.Parse(typeof(GacUtilCommands), value); }
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

        private ITaskItem[] _assemblies;
        

        /// <summary>
        /// Gets or sets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        [Required]
        public ITaskItem[] Assemblies
        {
            get { return _assemblies; }
            set { _assemblies = value; }
        } 
        #endregion
		
        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(
                ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version20),
                this.ToolName);
        }

        protected override string ToolName
        {
            get { return "gacutil.exe"; }
        }
    }
}
