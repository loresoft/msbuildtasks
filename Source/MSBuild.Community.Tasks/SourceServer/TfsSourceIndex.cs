using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.Tfs;

namespace MSBuild.Community.Tasks.SourceServer
{
    /// <summary>
    /// Task to index pdb files and entries to retrieve source files from Team Foundation Server source control.
    /// </summary>
    /// <remarks>
    /// This implementation is based on a pdb indexed by Team Foundation Build 2013 and has not been tested on other versions
    /// of TFS.
    /// </remarks>
    /// <example>Index a PDB.
    /// <code><![CDATA[
    /// <TfsSourceIndex SymbolFiles="@(Symbols)" TeamProjectCollectionUri="http://my-tfsserver/tfs/DefaultCollection" />
    /// ]]></code>
    /// </example>
    public class TfsSourceIndex : SourceIndexBase
    {
        /// <summary>
        /// Path to the root of the team collection hosting your project 
        /// </summary>
        /// <example>http://my-tfsserver/tfs/DefaultCollection</example>
        [Required]
        public string TeamProjectCollectionUri { get; set; }

        protected override bool AddSourceProperties(SymbolFile symbolFile)
        {
            var localInformation = GetLocalInformation(symbolFile.SourceFiles.Select(item => new TaskItem(item.File.FullName)).Cast<ITaskItem>());
            foreach (var item in symbolFile.SourceFiles)
            {
                var key = item.File.FullName.ToLower();
                if (!localInformation.ContainsKey(key))
                {
                    var allKeys = new StringBuilder();
                    foreach (var entry in localInformation)
                    {
                        allKeys.Append(entry.Key + "|");
                    }

                    throw new KeyNotFoundException(string.Format("Could not find a local information entry for |{0}|.\n{1}.", key, allKeys));
                }

                item.Properties["FileName"] = item.File.Name;
                item.Properties["Revision"] = localInformation[key].Changeset;
                item.Properties["ItemPath"] = localInformation[key].ServerPath.TrimStart('$');
                item.IsResolved = true;
            }

            return true;
        }

        protected override bool CreateSourceIndexFile(SymbolFile symbolFile, string sourceIndexFile)
        {
            using (var writer = File.CreateText(sourceIndexFile))
            {
                writer.WriteLine("SRCSRV: ini ------------------------------------------------");
                writer.WriteLine("VERSION=3");
                writer.WriteLine("INDEXVERSION=2");
                writer.WriteLine("VERCTRL=Team Foundation Server");
                writer.WriteLine("DATETIME={0}", DateTime.UtcNow.ToString("ddd MMM dd HH:mm:ss yyyy")); // strange format used by TFS, just copied in case its used
                writer.WriteLine("INDEXER=MSCT"); // MSBUILD Community tasks
                writer.WriteLine("SRCSRV: variables ------------------------------------------");
                writer.WriteLine(@"TFS_EXTRACT_CMD=tf.exe view /version:%var4% /noprompt ""$%var3%"" /server:%fnvar%(%var2%) /console >%srcsrvtrg%");
                writer.WriteLine(@"TFS_EXTRACT_TARGET=%targ%\%var2%%fnbksl%(%var3%)\%var4%\%fnfile%(%var5%)");
                writer.WriteLine("SRCSRVVERCTRL=tfs");
                writer.WriteLine("SRCSRVERRDESC=access");
                writer.WriteLine("SRCSRVERRVAR=var2");
                writer.WriteLine(string.Format("VSTFSSERVER={0}", this.TeamProjectCollectionUri));
                writer.WriteLine("SRCSRVTRG=%TFS_extract_target%");
                writer.WriteLine("SRCSRVCMD=%TFS_extract_cmd%");
                writer.WriteLine("SRCSRV: source files ---------------------------------------");

                foreach (var file in symbolFile.SourceFiles)
                    if (file.IsResolved)
                        writer.WriteLine(file.ToSourceString("{File}*VSTFSSERVER*{ItemPath}*{Revision}*{FileName}"));

                writer.WriteLine("SRCSRV: end ------------------------------------------------");

                writer.Flush();
                writer.Close();
            }

            return true;
        }

        private Dictionary<string, LocalInformation> GetLocalInformation(IEnumerable<ITaskItem> files)
        {
            TfsClient client = new TfsClient();
            CopyBuildEngine(client);
            client.Command = "info";
            client.Files = files.ToArray();

            client.Execute();
            
            var infoCommandResponse = new InfoCommandResponse(client.Output.ToString());
            return infoCommandResponse.LocalInformation;
        }


    }
}