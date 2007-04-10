#region Copyright © 2005 Paul Welter. All rights reserved.
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
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Globalization;

// $Id$

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Create a zip file with the files specified.
    /// </summary>
    /// <example>Create a zip file
    /// <code><![CDATA[
    /// <ItemGroup>
    ///     <ZipFiles Include="**\*.*" Exclude="*.zip" />
    /// </ItemGroup>
    /// <Target Name="Zip">
    ///     <Zip Files="@(ZipFiles)" 
    ///         ZipFileName="MSBuild.Community.Tasks.zip" />
    /// </Target>
    /// ]]></code>
    /// Create a zip file using a working directory.
    /// <code><![CDATA[
    /// <ItemGroup>
    ///     <RepoFiles Include="D:\svn\repo\**\*.*" />
    /// </ItemGroup>
    /// <Target Name="Zip">
    ///     <Zip Files="@(RepoFiles)" 
    ///         WorkingDirectory="D:\svn\repo" 
    ///         ZipFileName="D:\svn\repo.zip" />
    /// </Target>
    /// ]]></code>
    /// </example>
    public class Zip : Task
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Zip"/> class.
        /// </summary>
        public Zip()
        {
            _zipLevel = 6;
        }

        #endregion Constructor

        #region Input Parameters
        private string _zipFileName;

        /// <summary>
        /// Gets or sets the name of the zip file.
        /// </summary>
        /// <value>The name of the zip file.</value>
        [Required]
        public string ZipFileName
        {
            get { return _zipFileName; }
            set { _zipFileName = value; }
        }

        private int _zipLevel;

        /// <summary>
        /// Gets or sets the zip level.
        /// </summary>
        /// <value>The zip level.</value>
        /// <remarks>0 - store only to 9 - means best compression</remarks>
        public int ZipLevel
        {
            get { return _zipLevel; }
            set { _zipLevel = value; }
        }

        private ITaskItem[] _files;

        /// <summary>
        /// Gets or sets the files to zip.
        /// </summary>
        /// <value>The files to zip.</value>
        [Required]
        public ITaskItem[] Files
        {
            get { return _files; }
            set { _files = value; }
        }

        private bool _flatten;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Zip"/> is flatten.
        /// </summary>
        /// <value><c>true</c> if flatten; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Flattening the zip means that all directories will be removed 
        /// and the files will be place at the root of the zip file
        /// </remarks>
        public bool Flatten
        {
            get { return _flatten; }
            set { _flatten = value; }
        }

        private string _comment;

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        private string _workingDirectory;

        /// <summary>
        /// Gets or sets the working directory for the zip file.
        /// </summary>
        /// <value>The working directory.</value>
        /// <remarks>
        /// The working directory is the base of the zip file.  
        /// All files will be made relative from the working directory.
        /// </remarks>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }

        #endregion Input Parameters

        #region Task Overrides
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            return ZipFiles();
        }

        #endregion Task Overrides

        #region Private Methods
        private bool ZipFiles()
        {
            Crc32 crc = new Crc32();
            ZipOutputStream zs = null;

            try
            {
                Log.LogMessage(MSBuild.Community.Tasks.Properties.Resources.ZipCreating, _zipFileName);

                using (zs = new ZipOutputStream(File.Create(_zipFileName)))
                {

                    // make sure level in range
                    _zipLevel = System.Math.Max(0, _zipLevel);
                    _zipLevel = System.Math.Min(9, _zipLevel);

                    zs.SetLevel(_zipLevel);
                    if (!string.IsNullOrEmpty(_comment))
                        zs.SetComment(_comment);

                    byte[] buffer = new byte[32768];
                    // add files to zip
                    foreach (ITaskItem fileItem in _files)
                    {
                        string name = fileItem.ItemSpec;
                        FileInfo file = new FileInfo(name);
                        if (!file.Exists)
                        {
                            Log.LogWarning(MSBuild.Community.Tasks.Properties.Resources.FileNotFound, file.FullName);
                            continue;
                        }

                        // clean up name
                        if (_flatten)
                            name = file.Name;
                        else if (!string.IsNullOrEmpty(_workingDirectory)
                            && name.StartsWith(_workingDirectory, true, CultureInfo.InvariantCulture))
                            name = name.Remove(0, _workingDirectory.Length);

                        name = ZipEntry.CleanName(name);

                        ZipEntry entry = new ZipEntry(name);
                        entry.DateTime = file.LastWriteTime;
                        entry.Size = file.Length;

                        using (FileStream fs = file.OpenRead())
                        {
                            crc.Reset();
                            long len = fs.Length;
                            while (len > 0)
                            {
                                int readSoFar = fs.Read(buffer, 0, buffer.Length);
                                crc.Update(buffer, 0, readSoFar);
                                len -= readSoFar;
                            }
                            entry.Crc = crc.Value;
                            zs.PutNextEntry(entry);

                            len = fs.Length;
                            fs.Seek(0, SeekOrigin.Begin);
                            while (len > 0)
                            {
                                int readSoFar = fs.Read(buffer, 0, buffer.Length);
                                zs.Write(buffer, 0, readSoFar);
                                len -= readSoFar;
                            }
                        }
                        Log.LogMessage(MSBuild.Community.Tasks.Properties.Resources.ZipAdded, name);
                    } // foreach file
                    zs.Finish();
                }
                Log.LogMessage(MSBuild.Community.Tasks.Properties.Resources.ZipSuccessfully, _zipFileName);
                return true;
            }
            catch (Exception exc)
            {
                Log.LogErrorFromException(exc);
                return false;
            }
            finally
            {
                if (zs != null)
                    zs.Close();
            }
        }

        #endregion Private Methods
    }
}
