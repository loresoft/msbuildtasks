using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MSBuild.Community.Tasks.Tfs;

namespace MSBuild.Community.Tasks.SourceServer
{
    /// <summary>
    /// This implementation is based on the pdb indexed by Team Foundation Build 2013
    /// </summary>
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
                item.Properties["FileName"] = item.File.Name;
                item.Properties["Revision"] = localInformation[item.File.FullName].Changeset;
                item.Properties["ItemPath"] = localInformation[item.File.FullName].ServerPath.TrimStart('$');
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