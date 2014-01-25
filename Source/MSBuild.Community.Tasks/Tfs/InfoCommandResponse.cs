using System;
using System.Collections.Generic;

namespace MSBuild.Community.Tasks.Tfs
{
    public class InfoCommandResponse
    {
        public InfoCommandResponse(string output)
        {
            this.LocalInformation = new Dictionary<string, LocalInformation>();
            this.ServerInformation = new Dictionary<string, ServerInformation>();
            this.Response = output;
            this.Parse();
        }

        /// <summary>
        /// Parses the response from a 'tf.exe info' command
        /// </summary>
        /// <example>
        /// Local information:
        ///  Local path : c:\dev\file.cs
        ///  Server path: $/Main/file.cs
        ///  Changeset  : 5
        ///  Change     : none
        ///  Type       : file
        /// Server information:
        ///  Server path  : $/Main/file.cs
        ///  Changeset    : 5
        ///  Deletion ID  : 0
        ///  Lock         : none
        ///  Lock owner   :
        ///  Last modified: 20 January 2014 11:22:27
        ///  Type         : file
        ///  File type    : utf-8
        ///  Size         : 578           
        /// </example>
        private void Parse()
        {
            if (string.IsNullOrEmpty(this.Response))
                return;

            var lines = this.Response.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Contains("Local information:"))
                {
                    var localInformation = new LocalInformation
                    {
                        LocalPath = GetValue(lines[i + 1]),
                        ServerPath = GetValue(lines[i + 2]),
                        Changeset = GetValue(lines[i + 3]),
                        Change = GetValue(lines[i + 4]),
                        Type = GetValue(lines[i + 5]),
                    };

                    this.LocalInformation[localInformation.LocalPath] = localInformation;
                    i = i + 5;
                }
                else if (line.Contains("Server information:"))
                {
                    var serverInformation = new ServerInformation
                    {
                        ServerPath = GetValue(lines[i + 1]),
                        Changeset = GetValue(lines[i + 2]),
                        DeletionID  = GetValue(lines[i + 3]),
                        Lock = GetValue(lines[i + 4]),
                        LockOwner = GetValue(lines[i + 5]),
                        LastModified = GetValue(lines[i + 6]),
                        Type = GetValue(lines[i + 7]),
                        FileType  = GetValue(lines[i + 8]),
                        Size  = GetValue(lines[i + 9]),
                    };

                    this.ServerInformation[serverInformation.ServerPath] = serverInformation;
                    i = i + 9;
                }
            }
        }

        private string GetValue(string line)
        {
            var index = line.IndexOf(':');

            if (index == line.Length)
                return null;

            return line.Substring(index + 1).Trim();
        }

        public Dictionary<string, LocalInformation> LocalInformation { get; set; }

        public Dictionary<string, ServerInformation> ServerInformation { get; set; }
        public string Response { get; set; }
    }

    public class ServerInformation
    {
        public string ServerPath { get; set; }
        public string Changeset { get; set; }
        public string DeletionID { get; set; }
        public string Lock { get; set; }
        public string LockOwner { get; set; }
        public string LastModified { get; set; }
        public string Type { get; set; }
        public string FileType { get; set; }
        public string Size { get; set; }
    }

    public class LocalInformation
    {
        public string LocalPath { get; set; }
        public string ServerPath { get; set; }
        public string Changeset { get; set; }
        public string Change { get; set; }
        public string Type { get; set; }
    }
}