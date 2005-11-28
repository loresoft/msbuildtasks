// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.Globalization;

using Math = System.Math;

namespace MSBuild.Community.Tasks
{
    public class Zip : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Zip"/> class.
        /// </summary>
        public Zip()
        {
            _zipLevel = 6;
            _flatten = false;
        }

        private string _zipFile;

        /// <summary>
        /// Gets or sets the zip file name.
        /// </summary>
        /// <value>The zip file name.</value>
        [Required]
        public string ZipFile
        {
            get { return _zipFile; }
            set { _zipFile = value; }
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

        /// <summary>
        /// Searchs the files for a common base directory to start zip file
        /// </summary>
        /// <returns>A common directory</returns>
        private string FindBasePath()
        {
            string basePath = Path.GetDirectoryName(_files[0].ItemSpec);
            string[] folders = basePath.Split(Path.DirectorySeparatorChar);

            foreach (ITaskItem fileName in _files)
            {
                if (basePath.Length == 0)
                    break;

                string tempPath = Path.GetDirectoryName(fileName.ItemSpec);

                if (tempPath.StartsWith(basePath, true, CultureInfo.InvariantCulture))
                    continue;

                for (int i = folders.Length - 1; i > 0; i--)
                {
                    string baseTempPath = string.Join(Path.DirectorySeparatorChar.ToString(), folders, 0, i);
                    if (tempPath.StartsWith(baseTempPath, true, CultureInfo.InvariantCulture))
                    {
                        basePath = baseTempPath;
                        folders = baseTempPath.Split(Path.DirectorySeparatorChar);
                        break;
                    }
                }
            }

            return basePath + Path.DirectorySeparatorChar;
        }

        private bool ZipFiles()
        {
            Crc32 crc = new Crc32();
            ZipOutputStream zs = null;


            string basePath = FindBasePath();

            try
            {
                Log.LogMessage("Creating zip file \"{0}\".", _zipFile); 

                zs = new ZipOutputStream(File.Create(_zipFile));

                // make sure level in range
                _zipLevel = System.Math.Max(0, _zipLevel);
                _zipLevel = System.Math.Min(9, _zipLevel);

                zs.SetLevel(_zipLevel);
                if (!string.IsNullOrEmpty(_comment))
                    zs.SetComment(_comment);

                // add files to zip
                foreach (ITaskItem fileItem in _files)
                {
                    string name = fileItem.ItemSpec;
                    FileInfo file = new FileInfo(name);
                    if (!file.Exists)
                    {
                        Log.LogWarning("File not found: '{0}'", file.FullName);
                        continue;
                    }

                    byte[] buffer;

                    using (FileStream fs = file.OpenRead())
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();
                    }

                    // clean up name
                    if (_flatten)
                    {
                        name = file.Name;
                    }
                    else if (name.StartsWith(basePath, true, CultureInfo.InvariantCulture))
                    {
                        name = name.Remove(0, basePath.Length);
                    }
                    name = ZipEntry.CleanName(name, true);

                    ZipEntry entry = new ZipEntry(name);
                    entry.DateTime = file.LastWriteTime;
                    entry.Size = file.Length;

                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;

                    zs.PutNextEntry(entry);
                    zs.Write(buffer, 0, buffer.Length);

                    Log.LogMessage("File \"{0}\" added to zip file \"{1}\".", fileItem.ItemSpec, _zipFile);
                } // foreach file
                zs.Finish();
                return true;
            }
            finally
            {
                if (zs != null)
                    zs.Close();
            }
        }
    }
}
