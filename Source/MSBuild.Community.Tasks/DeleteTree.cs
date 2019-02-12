#region Copyright © 2008 MSBuild Community Task Project. All rights reserved.
/*
Copyright © 2008 MSBuild Community Task Project. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;




namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Delete a directory tree.  This task supports wild card directory selection.
    /// </summary>
    /// <example>
    /// <para>Delete all bin and obj directories.</para>
    /// <code><![CDATA[
    /// <DeleteTree Directories="**\bin;**\obj" />
    /// ]]></code>
    /// <para>Delete all bin and obj directories that start with MSBuild.Community.</para>
    /// <code><![CDATA[
    /// <DeleteTree Directories="MSBuild.Community.*\**\bin;MSBuild.Community.*\**\obj" />
    /// ]]></code>
    /// </example>
    public class DeleteTree : Task
    {
        private const string recursiveDirectoryMatch = "**";
        private static readonly char[] wildcardCharacters = new[] { '*', '?' };
        private static readonly char[] directorySeparatorCharacters = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        
        private const string recursiveRegex = @"(?:\\.*?)*";
        private const string anyCharRegex = @"[^\\]*?";
        private const string singleCharRegex = @"[^\\]";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteTree"/> class.
        /// </summary>
        public DeleteTree()
        {
            Recursive = true;
        }

        /// <summary>
        /// Gets or sets the directories to be deleted.
        /// </summary>
        /// <value>The directories to be deleted.</value>
        /// <remarks>
        /// Directories can contain wild cards.
        /// </remarks>
        [Required]
        public ITaskItem[] Directories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DeleteTree"/> is recursive.
        /// </summary>
        /// <value><c>true</c> if recursive; otherwise, <c>false</c>.</value>
        public bool Recursive { get; set; }

        private readonly List<ITaskItem> _deletedDirectories = new List<ITaskItem>();

        /// <summary>
        /// Gets the deleted directories.
        /// </summary>
        /// <value>The deleted directories.</value>
        [Output]
        public ITaskItem[] DeletedDirectories
        {
            get { return _deletedDirectories.ToArray(); }
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            foreach (var directory in Directories)
            {
                var matched = MatchDirectories(directory.ItemSpec);

                foreach (var dir in matched)
                {
                    _deletedDirectories.Add(new TaskItem(dir));

                    if (!Directory.Exists(dir))
                        continue;

                    Log.LogMessage("  Deleting Directory '{0}'", dir);
                    try
                    {
                        Directory.Delete(dir, Recursive);
                    }
                    catch (IOException ex) // continue to delete on the following exceptions
                    {
                        Log.LogErrorFromException(ex, false);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Log.LogErrorFromException(ex, false);
                    }
                }
            }

            return true;
        }

        internal static IList<string> MatchDirectories(string pattern)
        {
            var pathParts = pattern.Split(directorySeparatorCharacters, StringSplitOptions.RemoveEmptyEntries);

            var pathIndex = 0; // find root path with no wildcards
            var rootPath = FindRootPath(pathParts, out pathIndex);
            
            var directories = new List<string>(128);
            if (pathIndex >= pathParts.Length) // no wild cards or relative directories because there are no parts left, use root
            {
                directories.Add(rootPath);
                return directories;
            }

            // build up a regex to match on all child directories of root
            var pathRegex = new StringBuilder(Regex.Escape(rootPath));
            for (; pathIndex < pathParts.Length; pathIndex++)
            {
                if (pathParts[pathIndex] == recursiveDirectoryMatch)
                {
                    pathRegex.Append(recursiveRegex);
                    continue;
                }

                pathRegex.Append(@"\\");

                var part = pathParts[pathIndex];
                var partStart = 0;
                while (partStart < part.Length) // loop through part replacing wild with regex
                {
                    var wildIndex = part.IndexOfAny(wildcardCharacters, partStart);
                    if (wildIndex < 0)
                        wildIndex = part.Length;

                    // append chunk of part while escaping regex symbols
                    var s = Regex.Escape(part.Substring(partStart, wildIndex - partStart));
                    pathRegex.Append(s);

                    if (wildIndex < part.Length)
                    {
                        if (part[wildIndex] == '*')
                            pathRegex.Append(anyCharRegex);
                        else if (part[wildIndex] == '?')
                            pathRegex.Append(singleCharRegex);
                    }
                    partStart = wildIndex + 1;
                }       
            }
            pathRegex.Append("$");

            var searchRegex = new Regex(pathRegex.ToString(), RegexOptions.IgnoreCase);
            var dirs = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);
            
            directories.AddRange(dirs.Where(dir => searchRegex.IsMatch(dir)));

            return directories;
        }

        private static string FindRootPath(string[] parts, out int pathIndex)
        {
            var root = parts
                .TakeWhile(part => !part.Equals(recursiveDirectoryMatch))
                .TakeWhile(part => part.IndexOfAny(wildcardCharacters) < 0)
                .ToList();

            var rootPath = root.Any()
                ? Path.Combine(root.ToArray())
                : Environment.CurrentDirectory;

            if (!Path.IsPathRooted(rootPath))
                rootPath = Path.Combine(Environment.CurrentDirectory, rootPath);

            pathIndex = root.Count;
            return Path.GetFullPath(rootPath);
        }
    }
}
