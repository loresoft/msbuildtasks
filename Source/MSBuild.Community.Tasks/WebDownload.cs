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
using System.Net;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;


// $Id$

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Downloads a resource with the specified URI to a local file. 
    /// </summary>
    /// <example>Download the Microsoft.com home page.
    /// <code><![CDATA[
    /// <WebDownload FileUri="http://www.microsoft.com/default.aspx" 
    ///     FileName="microsoft.html" />
    /// ]]></code>
    /// </example>
    public class WebDownload : Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:WebDownload"/> class.
        /// </summary>
        public WebDownload()
        {

        }

        #region Properties
        private string _fileName;

        /// <summary>
        /// Gets or sets the name of the local file that is to receive the data.
        /// </summary>
        /// <value>The name of the file.</value>
        [Required]
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private string _fileUri;

        /// <summary>
        /// Gets or sets the URI from which to download data.
        /// </summary>
        /// <value>The file URI.</value>
        [Required]
        public string FileUri
        {
            get { return _fileUri; }
            set { _fileUri = value; }
        } 
        #endregion


        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            Log.LogMessage("Downloading File \"{0}\" from \"{1}\".", _fileName, _fileUri);

            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(_fileUri, _fileName);
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            Log.LogMessage("Successfully Downloaded File \"{0}\" from \"{1}\"", _fileName, _fileUri);
            return true;
        }
    }
}
