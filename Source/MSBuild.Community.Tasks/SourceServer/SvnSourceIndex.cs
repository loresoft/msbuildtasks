#region Copyright © 2009 MSBuild Community Task Project. All rights reserved.
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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.Subversion;

// $Id $

namespace MSBuild.Community.Tasks.SourceServer
{
    /// <summary>
    /// A subversion source index task.
    /// </summary>
    public class SvnSourceIndex : SourceIndexBase
    {
        private XmlSerializer _infoSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnSourceIndex"/> class.
        /// </summary>
        public SvnSourceIndex()
        {
            SourceTargetFormat = "%targ%\\%fnbksl%(%var3%)\\%var4%\\%fnfile%(%var1%)";
            SourceCommandFormat = "svn.exe export %var2%%var3%@%var4% %srcsrvtrg% --non-interactive --trust-server-cert --quiet %svnauth%";
        }

        /// <summary>
        /// Adds the source properties to the symbol file.
        /// </summary>
        /// <param name="symbolFile">The symbol file to add the source properties to.</param>
        /// <returns>
        /// 	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected override bool AddSourceProperties(SymbolFile symbolFile)
        {
            // get all the svn info in one call
            SvnClient client = new SvnClient();
            CopyBuildEngine(client);

            List<ITaskItem> items = new List<ITaskItem>();
            foreach (var sourceFile in symbolFile.SourceFiles)
                items.Add(new TaskItem(sourceFile.File.FullName));

            client.Command = "info";
            client.Xml = true;
            client.Targets = items.ToArray();

            if (!client.Execute())
            {
                Failed++;
                Log.LogError("Error getting source information from subversion.");
                return false;
            }

            Info info = null;

            // reuse for multiple calls. no need to make static, lifetime of msbuild.exe is short.
            if (_infoSerializer == null)
                _infoSerializer = new XmlSerializer(typeof(Info));

            using (var sr = new StringReader(client.Output))
            using (var reader = XmlReader.Create(sr))
            {
                info = _infoSerializer.Deserialize(reader) as Info;
            }

            if (info == null)
            {
                Failed++;
                Log.LogError("Error parsing svn info xml information.");
                return false;
            }

            foreach (var sourceFile in symbolFile.SourceFiles)
            {
                if (!info.Entries.Contains(sourceFile.File.FullName))
                {
                    Log.LogWarning("Information for source file '{0}' was not found.", sourceFile.File.FullName);
                    continue;
                }

                var entry = info.Entries[sourceFile.File.FullName];

                sourceFile.Properties["Revision"] = entry.Revision;

                string baseUri = entry.Repository.Root;
                if (!baseUri.EndsWith("/"))
                    baseUri += "/";

                sourceFile.Properties["Root"] = baseUri;
                sourceFile.Properties["Url"] = entry.Url;

                Uri root = new Uri(baseUri);
                Uri fullPath = new Uri(entry.Url);
                Uri itemPath = root.MakeRelativeUri(fullPath);

                sourceFile.Properties["ItemPath"] = itemPath.ToString();

                sourceFile.Properties["CommitRevision"] = entry.Commit.Revision;
                sourceFile.Properties["CommitAuthor"] = entry.Commit.Author;

                if (entry.Commit.DateSpecified)
                    sourceFile.Properties["CommitDate"] = entry.Commit.Date;

                sourceFile.Properties["Kind"] = entry.Kind;

                if (!string.Equals(entry.WorkingCopy.Schedule, "normal", StringComparison.OrdinalIgnoreCase))
                    Log.LogWarning("Source file '{0}' has pending changes. Index may point to incorrect revision.", sourceFile.File.FullName);                
                else if (entry.Revision == 0)
                    Log.LogWarning("Source file '{0}' has a revision of zero.", sourceFile.File.FullName);
                
                sourceFile.IsResolved = true;
            }

            return true;
        }

        /// <summary>
        /// Creates the source index file.
        /// </summary>
        /// <param name="symbolFile">The symbol file to create the index file from.</param>
        /// <param name="sourceIndexFile">The source index file.</param>
        /// <returns>
        /// 	<c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        protected override bool CreateSourceIndexFile(SymbolFile symbolFile, string sourceIndexFile)
        {
            using (var writer = File.CreateText(sourceIndexFile))
            {
                writer.WriteLine("SRCSRV: ini ------------------------------------------------");
                writer.WriteLine("VERSION=1");
                writer.WriteLine("INDEXVERSION=2");
                writer.WriteLine("VERCTRL=Subversion");
                writer.WriteLine("DATETIME={0}", DateTime.UtcNow.ToString("u"));
                writer.WriteLine("SRCSRV: variables ------------------------------------------");

                writer.WriteLine("SRCSRVTRG={0}", SourceTargetFormat);
                writer.WriteLine("SRCSRVCMD={0}", SourceCommandFormat);

                writer.WriteLine("SRCSRV: source files ---------------------------------------");

                foreach (var file in symbolFile.SourceFiles)
                    if (file.IsResolved)
                        writer.WriteLine(file.ToSourceString("{File}*{Root}*{ItemPath}*{Revision}"));

                writer.WriteLine("SRCSRV: end ------------------------------------------------");

                writer.Flush();
                writer.Close();
            }

            return true;
        }
    }
}
