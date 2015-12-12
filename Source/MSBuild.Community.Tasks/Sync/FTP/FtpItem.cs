/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2009 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;
using System.IO;
using System.Text;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// The itemType of item as reported by the FTP server.
    /// </summary>
    /// <remarks>
    /// Data transmitted from the FTP server after a directory list operation is usually a item itemType of Directory or File.  Unix 
    /// systems also support additional directory item types such as symbolic links and named sockets.  Not all FTP servers will report
    /// enough information to determine the file itemType.  In such cases a file itemType of Unknown is specified.
    /// </remarks>
    public enum FtpItemType
    {
        /// <summary>
        /// Directory item.
        /// </summary>
        Directory,
        /// <summary>
        /// File item.
        /// </summary>
        File,
        /// <summary>
        /// Symbolic link item.
        /// </summary>
        SymbolicLink,
        /// <summary>
        /// Block special file item.
        /// </summary>
        BlockSpecialFile,
        /// <summary>
        /// Character special file item.
        /// </summary>
        CharacterSpecialFile,
        /// <summary>
        /// Name socket item.
        /// </summary>
        NamedSocket,
        /// <summary>
        /// Domain socket item.
        /// </summary>
        DomainSocket,
        /// <summary>
        /// Unknown item.  The system was unable to determine the itemType of item.
        /// </summary>
        Unknown
    }


    /// <summary>
    /// The FtpItem class represents the file and directory listing items as reported by the FTP server.
    /// </summary>
    /// <remarks>
    /// Usually items are of types Files and Directories although
    /// the Unix based FTP servers may report additional information such as permissions and symbolic link information.  The FtpItem class supports
    /// the most commom versions of Unix, Windows, DOS, and Machintosh Ftp Servers.  There is no FTP standard concerning how an FTP server should
    /// list file item data.  Therefore, the FtpClient object supports a pluggable ftp item parser that you can write to support more exotic
    /// ftp item listing formats.
    /// </remarks>
	public class FtpItem 
	{
        private string _name;
        private DateTime _modified;
        private long _size;
        private string _symbolicLink;
        private FtpItemType _itemType;
        private string _attributes;
        private string _rawText;
        private string _parentPath;

        /// <summary>
        /// Constructor to create a new ftp item.
        /// </summary>
        /// <param name="name">Name of the item.</param>
        /// <param name="modified">Modified date and/or time of the item.</param>
        /// <param name="size">Number of bytes or size of the item.</param>
        /// <param name="symbolicLink">Symbolic link name.</param>
        /// <param name="attributes">Permission text for item.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="rawText">The raw text of the item.</param>
        public FtpItem(string name, DateTime modified, long size, string symbolicLink, string attributes, FtpItemType itemType, string rawText)
		{
            _name = name;
            _modified = modified;
            _size = size;
            _symbolicLink = symbolicLink;
            _attributes = attributes;
            _itemType = itemType;
            _rawText = rawText;
		}

        /// <summary>
        /// Item name.  All FTP servers should report a name value for the FTP item.
        /// </summary>
		public string Name
		{
			get	{ return _name;	}
		}

        /// <summary>
        /// Permissions text for the item.  Many FTP servers will report file permission information.
        /// </summary>
        public string Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
		/// Modified date and possibly time in the client's local time for the ftp item.
        /// </summary>
        public DateTime Modified
        {
            get { return _modified ; }
		}

		/// <summary>
		/// Modified date and possibly time in UTC time for the ftp item.
		/// </summary>
		public DateTime ModifiedUtc {
			get { return _modified.ToUniversalTime(); }
		}

        /// <summary>
        /// The size of the ftp item as reported by the FTP server.
        /// </summary>
        public long Size
        {
            get { return _size; }
        }

        /// <summary>
        /// The symbolic link name if the item is of itemType symbolic link.
        /// </summary>
        public string SymbolicLink
        {
            get { return _symbolicLink; }
        }

        /// <summary>
        /// The itemType of the ftp item.
        /// </summary>
        public FtpItemType ItemType
        {
            get { return _itemType; }
        }

        /// <summary>
        /// The raw textual line information as reported by the FTP server.  This can be useful for examining exotic FTP formats and for debugging
        /// a custom ftp item parser.
        /// </summary>
        public string RawText
        {
            get { return _rawText; }
        }

        /// <summary>
        /// Path to the parent directory.
        /// </summary>
        public string ParentPath
        {
            get { return _parentPath; }
            set { _parentPath = value; }
        }

        /// <summary>
        /// Item full path.
        /// </summary>
        public string FullPath
        {
            get { return _parentPath == "/" || _parentPath == "//" ? String.Format("{0}{1}", _parentPath, _name) : String.Format("{0}/{1}", _parentPath, _name); }
        }

		public void SetTimeOffset(int offset) {
			if (_modified.Kind == DateTimeKind.Local || _modified.Kind == DateTimeKind.Unspecified) {
				_modified = DateTime.SpecifyKind(_modified.AddHours(-offset), DateTimeKind.Utc).ToLocalTime();
			}
		}
	}
} 