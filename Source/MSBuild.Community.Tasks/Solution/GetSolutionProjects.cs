#region Copyright © 2006 Andy Johns. All rights reserved.
/*
Copyright © 2006 Andy Johns. All rights reserved.

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

//$Id$

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Task to get paths to projects and project names from VS2005 solution file
    /// </summary>
    /// <example>
    /// Returns project name and relative path from test solution
    /// <code><![CDATA[
    ///   <Target Name="Test">
    ///       <GetSolutionProjects Solution="TestSolution.sln">
    ///           <Output ItemName="ProjectFiles" TaskParameter="Output"/>
    ///       </GetSolutionProjects>
    /// 
    ///     <Message Text="Solution Project paths:" />
    ///     <Message Text="%(ProjectFiles.ProjectName) : @(ProjectFiles)" />
    ///   </Target>
    /// ]]></code>
    /// </example>
    public class GetSolutionProjects : Task
    {
        private const string ExtractProjectsFromSolutionRegex = @"""[\w\.]+"", ""[\w\.\\]+""";
        private readonly char[] SplitCharacters = new char[] { '\"', ',', ' '};
        private string solutionFile = "";
        private ITaskItem[] output = null;

        /// <summary>
        /// Output list contains TaskItems of project filenames contained within the given solution.
        /// Metadata tag "ProjectName" contains name of project.
        /// </summary>
        [Output]
        public ITaskItem[] Output
        {
            get { return output; }
            set { output = value; }
        }

        /// <summary>
        /// Name of Solution to get Projects from
        /// </summary>
        [Required]
        public string Solution
        {
            get { return solutionFile; }
            set { solutionFile = value; }
        }

        /// <summary>
        /// Perform task
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Execute()
        {
            if (!File.Exists(solutionFile))
            {
                Log.LogError(Properties.Resources.SolutionNotFound, solutionFile);
            }
            else
            {
                try
                {
                    string solutionText = "";
                    using (StreamReader sr = new StreamReader(solutionFile))
                    {
                        solutionText = sr.ReadToEnd();
                    }

                    Regex regex = new Regex(ExtractProjectsFromSolutionRegex);
                    MatchCollection matches = regex.Matches(solutionText);
                    output = new TaskItem[matches.Count];
                    for(int ndx=0; ndx<matches.Count; ndx++)
                    {
                        string[] parts = matches[ndx].Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                        output[ndx] = new TaskItem(parts[1]); 
                        output[ndx].SetMetadata("ProjectName", parts[0]);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex); 
                }
            }

            return !Log.HasLoggedErrors;
        }


    }
}
