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
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

// $Id$

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Replace text in file(s) using a Regular Expression.
    /// </summary>
    /// <example>Search for a version number and update the revision.
    /// <code><![CDATA[
    /// <FileUpdate Files="version.txt"
    ///     Regex="(\d+)\.(\d+)\.(\d+)\.(\d+)"
    ///     ReplacementText="$1.$2.$3.123" />
    /// ]]></code>
    /// </example>
    public class FileUpdate :Task
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FileUpdate"/> class.
        /// </summary>
        public FileUpdate()
        {

        }

        #region Properties

        private ITaskItem[] _files;

        /// <summary>
        /// Gets or sets the files to update.
        /// </summary>
        /// <value>The files.</value>
        public ITaskItem[] Files
        {
            get { return _files; }
            set { _files = value; }
        }

        private string _regex;

        /// <summary>
        /// Gets or sets the regex.
        /// </summary>
        /// <value>The regex.</value>
        public string Regex
        {
            get { return _regex; }
            set { _regex = value; }
        }

        private bool _ignoreCase;

        /// <summary>
        /// Gets or sets a value specifies case-insensitive matching. .
        /// </summary>
        /// <value><c>true</c> if [ignore case]; otherwise, <c>false</c>.</value>
        public bool IgnoreCase
        {
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        private bool _multiline;

        /// <summary>
        /// Gets or sets a value changing the meaning of ^ and $ so they match at the beginning and end, 
        /// respectively, of any line, and not just the beginning and end of the entire string.
        /// </summary>
        /// <value><c>true</c> if multiline; otherwise, <c>false</c>.</value>
        public bool Multiline
        {
            get { return _multiline; }
            set { _multiline = value; }
        }

        private bool _singleline;

        /// <summary>
        /// Gets or sets a value changing the meaning of the dot (.) so it matches 
        /// every character (instead of every character except \n). 
        /// </summary>
        /// <value><c>true</c> if singleline; otherwise, <c>false</c>.</value>
        public bool Singleline
        {
            get { return _singleline; }
            set { _singleline = value; }
        }

        private int _replacementCount = -1;

        /// <summary>
        /// Gets or sets the maximum number of times the replacement can occur.
        /// </summary>
        /// <value>The replacement count.</value>
        public int ReplacementCount
        {
            get { return _replacementCount; }
            set { _replacementCount = value; }
        }

        private string _replacementText;

        /// <summary>
        /// Gets or sets the replacement text.
        /// </summary>
        /// <value>The replacement text.</value>
        public string ReplacementText
        {
            get { return _replacementText; }
            set { _replacementText = value; }
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
            try
            {
                foreach (ITaskItem item in _files)
                {
                    string fileName = item.ItemSpec;

                    Log.LogMessage("Updating File \"{0}\".", fileName);

                    RegexOptions options = RegexOptions.None;
                    if (_ignoreCase)
                        options |= RegexOptions.IgnoreCase;
                    if (_multiline)
                        options |= RegexOptions.Multiline;
                    if (_singleline)
                        options |= RegexOptions.Singleline;

                    Regex replaceRegex = new Regex(_regex, options);

                    if (_replacementCount == 0)
                        _replacementCount = -1;

                    string buffer = File.ReadAllText(fileName);
                    buffer = replaceRegex.Replace(buffer, _replacementText, _replacementCount);
                    File.WriteAllText(fileName, buffer);

                    Log.LogMessage("  Replaced \"{0}\" with \"{1}\"", _regex, _replacementText);
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            return true;
        }
    }
}
