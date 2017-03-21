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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
//using Starksoft.Net.Proxy;

namespace Starksoft.Net.Ftp {

	#region Public Enums
	/// <summary>
	/// Enumeration representing type of file transfer mode.
	/// </summary>
	public enum TransferType : int {
		/// <summary>
		/// No transfer type.
		/// </summary>
		None,
		/// <summary>
		/// Transfer mode of type 'A' (ascii).
		/// </summary>
		Ascii,
		/// <summary>
		/// Transfer mode of type 'I' (image or binary)
		/// </summary>
		Binary // TYPE I
	}

	/// <summary>
	/// Enumeration representing the three types of actions that FTP supports when
	/// uploading or 'putting' a file on an FTP server from the FTP client.  
	/// </summary>
	public enum FileAction : int {
		/// <summary>
		/// No action.
		/// </summary>
		None,
		/// <summary>
		/// Create a new file or overwrite an existing file.
		/// </summary>
		Create,
		/// <summary>
		/// Create a new file.  Do not overwrite an existing file.
		/// </summary>
		CreateNew,
		/// <summary>
		/// Create a new file or append an existing file.
		/// </summary>
		CreateOrAppend,
		/// <summary>
		/// Resume a file transfer.
		/// </summary>
		Resume,
		/// <summary>
		/// Resume a file transfer if the file already exists.  Otherwise, create a new file.
		/// </summary>
		ResumeOrCreate
	}


	#endregion

	/// <summary>
	/// The FtpClient Component for .NET is a fully .NET coded RFC 959 compatible FTP object component that supports the RFC 959, SOCKS and HTTP proxies, SSLv2, SSLv3, and TLSv1
	/// as well as automatic file integrity checks on all data transfers.  
	/// The component library also supports a pluggable directory listing parser.  The Starksoft FtpClient Component for .NET support most FTP servers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The object implements and uses the following FTP commands and provides a simple to use component.
	/// FTP RFC 959 commands (and extended) directly supported: 
	///     USER    RMD     CDUP    CWD     STOU    RETR    AUTH    SITE CHMOD
	///     PASS    RETR    DELE    PORT    APPE    MDTM    PROT        
	///     QUIT    PWD     TYPE    PASV    REST    SIZE    MODE  
	///     MKD     SYST    MODE    STOR    RNFR    FEAT    XSHA1
	///     NLST    HELP    RNTO    SITE    ALLO    QUIT    XMD5
	///     ABORT   STAT    LIST    NOOP    PBSZ    XCRC    
	/// </para>
	/// <para>
	/// Custom FTP server commands can be executed using the Quote() method.  This allows the FtpClient object to handle
	/// certain custom commands that are not supported by the RFC 959 standard but are required by specific FTP server
	/// implementations for various tasks.
	/// </para>
	/// <para>
	/// The Starksoft FtpClient Component for .NET supports SOCKS v4, SOCKS v4a, SOCKS v5, and HTTP proxy servers.  The proxy settings are not read
	/// from the local web browser proxy settings so deployment issues are not a problem with using proxy connections.  In addition the library also
	/// supports active and passive (firewall friendly) mode transfers.  The Starksoft FtpClient Component for .NET supports data compression, bandwidth throttling,
	/// and secure connections through SSL (Secure Socket Layer) and TLS.  The Starksoft FtpClient Component for .NET also supports automatic transfer integrity checks via 
	/// CRC, MD5, and SHA1.  The FtpClient object can parse many different directory listings from various FTP server implementations.  But for those servers that are difficult to 
	/// parse of produce strange directory listings you can write your own ftp item parser.  See the IFtpItemParser interface
	/// for more information and an example parser.  Finally, the Starksoft FtpClient Component for .NET  also provides support for encrypting and decrypting PGP data files though a .NET wrapper
	/// class that interfaces directly with the open source GNU Open PGP executable.   
	/// </para>
	/// <para>
	/// The FtpClient libary has been tested with the following FTP servers and file formats.
	/// <list type="">
	///     <item>IIS 6.0 under Microsoft Windows 2000 and Windows 2003 server, </item>
	///     <item>Microsoft FTP server running IIS 5.0</item>
	///     <item>Gene6FTP Server</item>
	///     <item>ProFTPd</item>
	///     <item>Wu-FTPd</item>
	///     <item>WS_FTP Server (by Ipswitch)</item>
	///     <item>Serv-U FTP Server</item>
	///     <item>GNU FTP server</item>
	///     <item>Many public FTP servers</item>
	/// </list>
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// FtpClient ftp = new FtpClient("ftp.microsoft.com");
	/// // note: DataTransferMode is actually passive mode (PASV) by default
	/// ftp.DataTransferMode = DataTransferMode.Passive; 
	/// ftp.Open("anonymous", "myemail@host.com");
	/// ftp.ChangeDirectory("Softlib");
	/// ftp.GetFile("README.TXT", "c:\\README.TXT"); 
	/// ftp.Close();
	/// </code>
	/// </example>
	public class FtpClient : FtpBase {
		#region Contructors

		/// <summary>
		/// FtpClient default constructor.
		/// </summary>
		public FtpClient()
			: base(DEFAULT_FTP_PORT, FtpSecurityProtocol.None) { }

		/// <summary>
		/// Constructor method for FtpClient.  
		/// </summary>
		/// <param name="host">String containing the host name or ip address of the remote FTP server.</param>
		/// <remarks>
		/// This method takes one parameter to specify
		/// the host name (or ip address).
		/// </remarks>
		public FtpClient(string host)
			: this(host, DEFAULT_FTP_PORT, FtpSecurityProtocol.None) { }

		/// <summary>
		/// Constructor method for FtpClient.  
		/// </summary>
		/// <param name="host">String containing the host name or ip address of the remote FTP server.</param>
		/// <param name="port">Port number used to connect to the remote FTP server.</param>
		/// <remarks>
		/// This method takes two parameters that specify 
		/// the host name (or ip address) and the port to connect to the host.
		/// </remarks>
		public FtpClient(string host, int port)
			: base(host, port, FtpSecurityProtocol.None) { }

		/// <summary>
		/// Constructor method for FtpClient.  
		/// </summary>
		/// <param name="host">String containing the host name or ip address of the remote FTP server.</param>
		/// <param name="port">Port number used to connect to the remote FTP server.</param>
		/// <param name="securityProtocol">Enumeration value indicating what security protocol (such as SSL) should be enabled for this connection.</param>
		/// <remarks>
		/// This method takes three parameters that specify 
		/// the host name (or ip address), port to connect to and what security protocol should be used when establishing the connection.
		/// </remarks>
		public FtpClient(string host, int port, FtpSecurityProtocol securityProtocol)
			: base(host, port, securityProtocol) { }

		#endregion

		#region Private Variables and Constants

		private const int DEFAULT_FTP_PORT = 21; // default port is 21
		private const int FXP_TRANSFER_TIMEOUT = 600000; // 10 minutes

		private TransferType _fileTransferType = TransferType.Binary;
		private IFtpItemParser _itemParser;
		private string _user;
		private string _password;
		private bool _opened;
		private string _currentDirectory;
		private int _fxpTransferTimeout = FXP_TRANSFER_TIMEOUT;

		// transfer log
		private Stream _log = new MemoryStream();
		private bool _isLoggingOn;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the file transfer item.
		/// </summary>
		public TransferType FileTransferType {
			get { return _fileTransferType; }
			set {
				_fileTransferType = value;

				if (this.IsConnected == true) {
					//  update the server with the new file transfer type
					SetFileTransferType();
				}
			}
		}

		/// <summary>
		/// Gets or sets the the directory item parser to use when parsing directory listing data from the FTP server.
		/// This parser is used by the GetDirList() and GetDirList(string) methods.  
		/// </summary>
		/// <remarks>
		/// You can create your own custom directory listing parser by creating an object that implements the 
		/// IFtpItemParser interface.  This is particular useful when parsing exotic file directory listing
		/// formats from specific FTP servers.
		/// </remarks>
		public IFtpItemParser ItemParser {
			get { return _itemParser; }
			set { _itemParser = value; }
		}

		/// <summary>
		/// Gets or sets logging of file transfers.
		/// </summary>
		/// <remarks>
		/// All data transfer activity can be retrieved from the Log property.
		/// </remarks>
		public bool IsLoggingOn {
			get { return _isLoggingOn; }
			set { _isLoggingOn = value; }
		}

		/// <summary>
		/// Gets or sets the Stream object used for logging data transfer activity.
		/// </summary>
		/// <remarks>
		/// By default a MemoryStream object is created to log all data transfer activity.  Any 
		/// Stream object that can be written to may be used in place of the MemoryStream object.
		/// </remarks>
		/// <seealso cref="IsLoggingOn"/>
		public Stream Log {
			get { return _log; }
			set {
				if (((Stream)value).CanWrite == false)
					throw new ArgumentException("must be writable. The property CanWrite must have a value equals to 'true'.", "value");
				_log = value;
			}
		}

		/// <summary>
		/// Gets or sets the timeout value in miliseconds when waiting for an FXP server to server transfer to complete.
		/// </summary>
		/// <remarks>By default this timeout value is set to 600000 (10 minutes).  For large FXP file transfers you may need to adjust this number higher.</remarks>
		public int FxpTransferTimeout {
			get { return _fxpTransferTimeout; }
			set { _fxpTransferTimeout = value; }
		}

		/// <summary>
		/// Gets the current directory path without sending having to send a request to the server.
		/// </summary>
		/// <seealso cref="GetWorkingDirectory"/>
		public string CurrentDirectory {
			get { return _currentDirectory; }
		}


		private int? FindFileTimeOffset(string path) {
			if (!string.IsNullOrEmpty(path)) ChangeDirectory(path);
			var items = GetDirList();
			var item = items.FirstOrDefault(x => x.ItemType == FtpItemType.File);
			if (item != null) {
				var t = GetFileDateTime(item.Name, false);
				return (int?)((item.Modified - t).TotalHours + 0.5);
			}
			foreach (var dir in items.Where(x => x.ItemType == FtpItemType.Directory)) {
				var offset = FindFileTimeOffset(dir.FullPath);
				if (offset.HasValue) return offset;
			}
			return null;
		}

		public int? TimeOffset = null;
		public int ServerTimeOffset {
			get {
				if (!TimeOffset.HasValue) {
					TimeOffset = (int)((DateTime.Now - DateTime.UtcNow).TotalHours + 0.5);
					TimeOffset = FindFileTimeOffset(null) ?? 0;
				}
				return TimeOffset.Value;
			}
			set { TimeOffset = value; }
		}

		public string ServerTimeString {
			get {
				var offset = ServerTimeOffset;
				string zone;
				if (offset == 0) zone = "Z";
				else {
					zone = offset.ToString();
					if (offset > 0) zone = "+" + zone;
					else zone = "-" + zone;
				}
				return DateTime.UtcNow.AddHours(offset).ToShortTimeString() + zone;
			}
		}
		#endregion

		#region Public Methods

		public virtual string RootDirectory { get; set; }
		/// <summary>
		/// Opens a connection to the remote FTP server and log in with user name and password credentials.
		/// </summary>
		/// <param name="user">User name.  Many public ftp allow for this value to 'anonymous'.</param>
		/// <param name="password">Password.  Anonymous public ftp servers generally require a valid email address for a password.</param>
		/// <remarks>Use the Close() method to log off and close the connection to the FTP server.</remarks>
		/// <seealso cref="OpenAsync"/>
		/// <seealso cref="Close"/>
		/// <seealso cref="Reopen"/>
		public virtual void Open(string user, string password) {
			if (user == null)
				throw new ArgumentNullException("user", "must have a value");

			if (user.Length == 0)
				throw new ArgumentException("must have a value", "user");

			if (password == null)
				throw new ArgumentNullException("password", "must have a value or an empty string");

			// if the command connection is no already open then open a new command connect
			if (!this.IsConnected)
				base.OpenCommandConn();

			// test to see if this is an asychronous operation and if so make sure 
			// the user has not requested the operation to be canceled
			if (base.AsyncWorker != null && base.AsyncWorker.CancellationPending) {
				base.CloseAllConnections();
				return;
			}

			_user = user;
			_password = password;
			_currentDirectory = "/";

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.User, user));
			} catch (FtpException fex) {
				throw new FtpConnectionOpenException(String.Format("An error occurred when sending user information.  Reason: {0}", base.LastResponse.Text), fex);
			}

			// wait for user to log into system and all response messages to be transmitted
			Thread.Sleep(500);

			// test to see if this is an asychronous operation and if so make sure 
			// the user has not requested the operation to be canceled
			if (base.AsyncWorker != null && base.AsyncWorker.CancellationPending) {
				base.CloseAllConnections();
				return;
			}

			// some ftp servers do not require passwords for users and will log you in immediately - no password command is required
			if (base.LastResponse.Code != FtpResponseCode.UserLoggedIn) {
				try {
					base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Pass, password));
				} catch (FtpException fex) {
					throw new FtpConnectionOpenException(String.Format("An error occurred when sending password information.  Reason: {0}", base.LastResponse.Text), fex);
				}

				if (base.LastResponse.Code == FtpResponseCode.NotLoggedIn)
					throw new FtpLoginException("Unable to log into FTP destination with supplied username and password.");
			}

			// test to see if this is an asychronous operation and if so make sure 
			// the user has not requested the operation to be canceled
			if (base.AsyncWorker != null && base.AsyncWorker.CancellationPending) {
				base.CloseAllConnections();
				return;
			}

			// if the custom item parser is not set then set to use the built-in generic parser
			if (_itemParser == null)
				_itemParser = new FtpGenericParser();

			//  set the file type used for transfers
			SetFileTransferType();

			// if compression is indicated then send the compression command
			if (base.IsCompressionEnabled)
				base.CompressionOn();

			// test to see if this is an asychronous operation and if so make sure 
			// the user has not requested the operation to be canceled
			if (base.AsyncWorker != null && base.AsyncWorker.CancellationPending) {
				base.CloseAllConnections();
				return;
			}

			_opened = true;
			_currentDirectory = GetWorkingDirectory();
			RootDirectory = _currentDirectory;
		}

		/// <summary>
		/// Reopens a lost ftp connection.
		/// </summary>
		/// <remarks>
		/// If the connection is currently open or the connection has never been open and FtpException is thrown.
		/// </remarks>
		public virtual void Reopen() {
			if (!_opened)
				throw new FtpException("You must use the Open() method before using the Reopen() method.");

			// reopen the connection with the same username and password
			Open(_user, _password);
		}

		/// <summary>
		/// Change the currently logged in user to another user on the FTP server.
		/// </summary>
		/// <param name="user">The name of user.</param>
		/// <param name="password">The password for the user.</param>
		public virtual void ChangeUser(string user, string password) {
			if (user == null)
				throw new ArgumentNullException("user", "must have a value");

			if (user.Length == 0)
				throw new ArgumentException("must have a value", "user");

			if (password == null)
				throw new ArgumentNullException("password", "must have a value");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.User, user));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred when sending user information.", base.LastResponse, fex);
			}

			// wait for user to log into system and all response messages to be transmitted
			Thread.Sleep(500);

			// test to see if this is an asychronous operation and if so make sure 
			// the user has not requested the operation to be canceled
			if (base.AsyncWorker != null && base.AsyncWorker.CancellationPending) {
				base.CloseAllConnections();
				return;
			}

			// some ftp servers do not require passwords for users and will log you in immediately - no password command is required
			if (base.LastResponse.Code != FtpResponseCode.UserLoggedIn) {
				try {
					base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Pass, password));
				} catch (FtpException fex) {
					throw new FtpException("An error occurred when sending password information.", base.LastResponse, fex);
				}

				if (base.LastResponse.Code == FtpResponseCode.NotLoggedIn)
					throw new FtpLoginException("Unable to log into FTP destination with supplied username and password.");
			}
		}

		public virtual string CorrectPath(string path) {
			if (path.StartsWith("/")) {
				if (RootDirectory.EndsWith("/")) path = RootDirectory + path.Substring(1);
				else path = RootDirectory + path;
			}
			return path;
		}
		/// <summary>
		/// Closes connection to the FTP server.
		/// </summary>
		/// <seealso cref="FtpBase.ConnectionClosed"/>
		/// <seealso cref="Reopen"/>
		/// <seealso cref="Open"/>
		public virtual void Close() {
			base.CloseAllConnections();
		}

		/// <summary>
		/// Changes the current working directory on older FTP servers that cannot handle a full path containing
		/// multiple subdirectories.  This method will separate the full path into separate change directory commands
		/// to support such systems.
		/// </summary>
		/// <param name="path">Path of the new directory to change to.</param>
		/// <remarks>Accepts both foward slash '/' and back slash '\' path names.</remarks>
		/// <seealso cref="ChangeDirectory"/>
		/// <seealso cref="GetWorkingDirectory"/>
		public virtual void ChangeDirectoryMultiPath(string path) {
			// the change working dir command can generally handle all the weird directory name spaces
			// which is nice but frustrating that the ftp server implementors did not fix it for other commands

			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			// replace the windows style directory delimiter with a unix style delimiter
			path = path.Replace("\\", "/");

			char[] splitter = new char[] { '/' };

			var dirs = path.Split(splitter, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> cdirs = new List<string>();

			if (path.StartsWith("/")) {
				_currentDirectory = GetWorkingDirectory();
				cdirs = (_currentDirectory ?? "/").Split(splitter, StringSplitOptions.RemoveEmptyEntries).ToList();
				while (dirs.Count > 0 && cdirs.Count > 0 && dirs[0] == cdirs[0]) {
					dirs.RemoveAt(0); cdirs.RemoveAt(0);
				}
			}

			try {
				// issue a single CWD command for each directory
				// this is a very reliable method to change directories on all FTP servers
				// because some systems do not all a full path to be specified when changing directories
				var n = cdirs.Count;
				while (n-- > 0) ChangeDirectoryUp();
				foreach (string dir in dirs) {
					base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Cwd, dir));
				}
			} catch (FtpException fex) {
				throw new FtpException(String.Format("Could not change working directory to '{0}'.", path), fex);
			} finally {
				_currentDirectory = GetWorkingDirectory();
			}
		}

		/// <summary>
		/// Changes the current working directory on the server.  Some FTP server will not accept this command 
		/// if the path contains mutiple directories.  For those FTP server implementations see the method
		/// ChangeDirectoryMultiPath(string).
		/// </summary>
		/// <param name="path">Path of the new directory to change to.</param>
		/// <remarks>Accepts both foward slash '/' and back slash '\' path names.</remarks>
		/// <seealso cref="ChangeDirectoryMultiPath(string)"/>
		/// <seealso cref="GetWorkingDirectory"/>
		public virtual void ChangeDirectory(string path) {
			// the change working dir command can generally handle all the weird directory name spaces
			// which is nice but frustrating that the ftp server implementors did not fix it for other commands

			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			// replace the windows style directory delimiter with a unix style delimiter
			path = path.Replace("\\", "/");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Cwd, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("Could not change working directory to '{0}'.", path), fex);
			} finally {
				_currentDirectory = GetWorkingDirectory();
			}
		}


		/// <summary>
		/// Gets the current working directory.
		/// </summary>
		/// <returns>A string value containing the current full working directory path on the FTP server.</returns>
		/// <seealso cref="ChangeDirectory"/>
		/// <seealso cref="ChangeDirectoryUp"/>
		public virtual string GetWorkingDirectory() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Pwd));
			} catch (FtpException fex) {
				throw new FtpException("Could not retrieve working directory.", base.LastResponse, fex);
			}

			//  now we have to fix the directory due to formatting
			//  most ftp servers send something like this:  257 "/awg/inbound" is current directory.
			string dir = base.LastResponse.Text;

			//  if the pwd is in quotes, then extract it
			if (dir.Substring(0, 1) == "\"")
				dir = dir.Substring(1, dir.IndexOf("\"", 1) - 1);

			return dir;
		}

		/// <summary>
		/// Deletes a file on the remote FTP server.  
		/// </summary>
		/// <param name="path">The path name of the file to delete.</param>
		/// <remarks>
		/// The file is deleted in the current working directory if no path information 
		/// is specified.  Otherwise a full absolute path name can be specified.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to delete the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="DeleteDirectory"/>
		public virtual void DeleteFile(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");


			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Dele, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("Unable to the delete file {0}.", path), base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Aborts an action such as transferring a file to or from the server.  
		/// </summary>
		/// <remarks>
		/// The abort command is sent up to the server signaling the server to abort the current activity.
		/// </remarks>
		public virtual void Abort() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Abor));
			} catch (FtpException fex) {
				throw new FtpException("Abort command failed or was unable to be issued.", base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Creates a new directory on the remote FTP server.  
		/// </summary>
		/// <param name="path">The name of a new directory or an absolute path name for a new directory.</param>
		/// <remarks>
		/// If a directory name is given for path then the server will create a new subdirectory 
		/// in the current working directory.  Optionally, a full absolute path may be given.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to make the subdirectory using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		public virtual void MakeDirectory(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must contain a value", "path");


			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Mkd, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("The directory {0} could not be created.", path), base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Moves a file on the remote FTP server from one directory to another.  
		/// </summary>
		/// <param name="fromPath">Path and/or file name to be moved.</param>
		/// <param name="toPath">Destination path specifying the directory where the file will be moved to.</param>
		/// <remarks>
		/// This method actually results in several FTP commands being issued to the server to perform the physical file move.  
		/// This method is available for your convenience when performing common tasks such as moving processed files out of a pickup directory
		/// and into a archive directory.
		/// Note that some older FTP server implementations will not accept a full path to a filename.  On those systems this method may not work
		/// properly.
		/// </remarks>
		public virtual void MoveFile(string fromPath, string toPath) {
			if (fromPath == null)
				throw new ArgumentNullException("fromPath");

			if (fromPath.Length == 0)
				throw new ArgumentException("must contain a value", "fromPath");

			if (toPath == null)
				throw new ArgumentNullException("toPath");

			if (fromPath.Length == 0)
				throw new ArgumentException("must contain a value", "toPath");

			//  retrieve the server file from the current working directory
			MemoryStream fileStream = new MemoryStream();
			GetFile(fromPath, fileStream, false);

			//  create the remote file in the new location
			this.PutFile(fileStream, toPath, FileAction.Create);

			//  delete the original file from the original location
			this.DeleteFile(fromPath);
		}

		/// <summary>
		/// Deletes a directory from the FTP server.
		/// </summary>
		/// <param name="path">Directory to delete.</param>
		/// <remarks>
		/// The path can be either a specific subdirectory relative to the 
		/// current working directory on the server or an absolute path to 
		/// the directory to remove.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the parent directory of the directory you wish to delete using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="DeleteFile"/>
		public virtual void DeleteDirectory(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Rmd, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("The FTP destination was unable to delete the directory '{0}'.", path), base.LastResponse, fex);
			}

		}
		/// <summary>
		/// Executes the specific help dialog on the FTP server.  
		/// </summary>
		/// <returns>
		/// A string contains the help dialog from the FTP server.
		/// </returns>
		/// <remarks>
		/// Every FTP server supports a different set of commands and this commands 
		/// can be obtained by the FTP HELP command sent to the FTP server.  The information sent
		/// back is not parsed or processed in any way by the FtpClient object.  
		/// </remarks>
		public virtual string GetHelp() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Help));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred while getting the system help.", base.LastResponse, fex);
			}

			return base.LastResponse.Text;
		}

		/// <summary>
		/// Retrieves the data and time for a specific file on the ftp server as a Coordinated Universal Time (UTC) value (formerly known as GMT). 
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="adjustToLocalTime">Specifies if modified date and time as reported on the FTP server should be adjusted to the local time zone with daylight savings on the client.</param>
		/// <returns>
		/// A date time value.
		/// </returns>
		/// <remarks>
		/// This function uses the MDTM command which is an additional feature command and therefore not supported
		/// by all FTP servers.
		/// </remarks>
		/// <seealso cref="GetFileSize"/>
		public virtual DateTime GetFileDateTime(string fileName, bool adjustToLocalTime) {
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			if (fileName.Length == 0)
				throw new ArgumentException("must contain a value", "fileName");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Mdtm, fileName));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred when retrieving file date and time for '{0}'.", fileName), base.LastResponse, fex);
			}

			string response = base.LastResponse.Text;

			int year = int.Parse(response.Substring(0, 4), CultureInfo.InvariantCulture);
			int month = int.Parse(response.Substring(4, 2), CultureInfo.InvariantCulture);
			int day = int.Parse(response.Substring(6, 2), CultureInfo.InvariantCulture);
			int hour = int.Parse(response.Substring(8, 2), CultureInfo.InvariantCulture);
			int minute = int.Parse(response.Substring(10, 2), CultureInfo.InvariantCulture);
			int second = int.Parse(response.Substring(12, 2), CultureInfo.InvariantCulture);

			DateTime dateUtc = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

			if (adjustToLocalTime)
				return new DateTime(dateUtc.ToLocalTime().Ticks);
			else
				return new DateTime(dateUtc.Ticks);
		}

		/// <summary>
		/// Set the date and time for a specific file or directory on the server.
		/// </summary>
		/// <param name="path">The path or name of the file or directory.</param>
		/// <param name="dateTime">New date to set on the file or directory.</param>
		/// <remarks>
		/// This function uses the MDTM command which is an additional feature command and therefore not supported
		/// by all FTP servers.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory which has the file you wish to set the date and time using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="Rename"/>
		public virtual void SetDateTime(string path, DateTime dateTime) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			// MDTM [YYMMDDHHMMSS] [filename]

			string dateTimeArg = dateTime.ToUniversalTime().ToString("yyyyMMddHHmmss");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Mdtm, dateTimeArg, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred when setting a file date and time for '{0}'.", path), fex);
			}
		}

		/// <summary>
		/// Retrieves the specific status for the FTP server.  
		/// </summary>
		/// <remarks>
		/// Each FTP server may return different status dialog information.  The status information sent
		/// back is not parsed or processed in any way by the FtpClient object. 
		/// </remarks>
		/// <returns>
		/// A string containing the status of the FTP server.
		/// </returns>
		public virtual string GetStatus() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Stat));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred while getting the system status.", base.LastResponse, fex);
			}

			return base.LastResponse.Text;
		}

		/// <summary>
		/// Changes the current working directory on the FTP server to the parent directory.  
		/// </summary>
		/// <remarks>
		/// If there is no parent directory then ChangeDirectoryUp() will not have 
		/// any affect on the current working directory.
		/// </remarks>
		/// <seealso cref="ChangeDirectory"/>
		/// <seealso cref="GetWorkingDirectory"/>
		public virtual void ChangeDirectoryUp() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Cdup));
				int i = _currentDirectory.LastIndexOf('/', _currentDirectory.Length - 2);
				_currentDirectory = _currentDirectory.Substring(0, i);

			} catch (FtpException fex) {
				throw new FtpException("An error occurred when changing directory to the parent (ChangeDirectoryUp).", base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Get the file size for a file on the remote FTP server.  
		/// </summary>
		/// <param name="path">The name and/or path to the file.</param>
		/// <returns>An integer specifying the file size.</returns>
		/// <remarks>
		/// The path can be file name relative to the current working directory or an absolute path.  This command is an additional feature 
		/// that is not supported by all FTP servers.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to get the file size using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="GetFileDateTime"/>
		public virtual long GetFileSize(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must contain a value", "path");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Size, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred when retrieving file size for {0}.", path), base.LastResponse, fex);
			}

			return Int64.Parse(base.LastResponse.Text, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Get the additional features supported by the remote FTP server.  
		/// </summary>
		/// <returns>A string containing the additional features beyond the RFC 959 standard supported by the FTP server.</returns>
		/// <remarks>
		/// This command is an additional feature beyond the RFC 959 standard and therefore is not supported by all FTP servers.        
		/// </remarks>
		public virtual string GetFeatures() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Feat));
				//return base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.Feat));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred when retrieving features.", base.LastResponse, fex);
			}
			var sb = new StringBuilder();
			for (int i = 1; i < base.LastResponseList.Count - 1; i++) sb.AppendLine(base.LastResponseList[i].RawText.Trim());

			return sb.ToString();
		}

		/// <summary>
		/// Retrieves the specific status for a file on the FTP server.  
		/// </summary>
		/// <param name="path">
		/// The path to the file.
		/// </param>
		/// <returns>
		/// A string containing the status for the file.
		/// </returns>
		/// <remarks>
		/// Each FTP server may return different status dialog information.  The status information sent
		/// back is not parsed or processed in any way by the FtpClient object. 
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to get the status of the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		public virtual string GetStatus(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must contain a value", "path");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Stat, path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred when retrieving file status for file '{0}'.", path), base.LastResponse, fex);
			}

			return base.LastResponse.Text;
		}

		/// <summary>
		/// Allocates storage for a file on the FTP server prior to data transfer from the FTP client.  
		/// </summary>
		/// <param name="size">
		/// The storage size to allocate on the FTP server.
		/// </param>
		/// <remarks>
		/// Some FTP servers may return the client to specify the storage size prior to data transfer from the FTP client to the FTP server.
		/// </remarks>
		public virtual void AllocateStorage(long size) {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Stor, size.ToString()));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred when trying to allocate storage on the destination.", base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Retrieves a string identifying the remote FTP system.  
		/// </summary>
		/// <returns>
		/// A string contains the server type.
		/// </returns>
		/// <remarks>
		/// The string contains the word "Type:", and the default transfer type 
		/// For example a UNIX FTP server will return 'UNIX Type: L8'.  A Windows 
		/// FTP server will return 'WINDOWS_NT'.
		/// </remarks>
		public virtual string GetSystemType() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Syst));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred while getting the system type.", base.LastResponse, fex);
			}

			return base.LastResponse.Text;
		}

		/// <summary>
		/// Uploads a local file specified in the path parameter to the remote FTP server.  
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server.  
		/// A unique file name is created by the server.    
		/// </remarks>
		public virtual void PutFileUnique(string localPath) {
			if (localPath == null)
				throw new ArgumentNullException("localPath");

			try {
				using (FileStream fileStream = File.OpenRead(localPath)) {
					PutFileUnique(fileStream);
				}
			} catch (FtpException fex) {
				WriteToLog(String.Format("Action='PutFileUnique';Action='TransferError';LocalPath='{0}';CurrentDirectory='{1}';ErrorMessage='{2}'", localPath, _currentDirectory, fex.Message));
				throw new FtpException("An error occurred while executing PutFileUnique() on the remote FTP destination.", base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Uploads any stream object to the remote FTP server and stores the data under a unique file name assigned by the FTP server.  
		/// </summary>
		/// <param name="inputStream">Any stream object on the local client machine.</param>
		/// <remarks>
		/// The stream is uploaded to the current working directory on the remote server.  
		/// A unique file name is created by the server to store the data uploaded from the stream.
		/// </remarks>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="GetFile(string, string)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>  
		public virtual void PutFileUnique(Stream inputStream) {
			if (inputStream == null)
				throw new ArgumentNullException("inputStream");

			if (!inputStream.CanRead)
				throw new ArgumentException("must be readable.  The CanRead property must return a value of 'true'.", "inputStream");

			WriteToLog(String.Format("Action='PutFileUnique';Action='TransferBegin';CurrentDirectory='{0}'", _currentDirectory));

			try {
				base.TransferData(TransferDirection.ToServer, new FtpRequest(base.CharacterEncoding, FtpCmd.Stor), inputStream);
			} catch (Exception ex) {
				WriteToLog(String.Format("Action='PutFileUnique';Action='TransferError';CurrentDirectory='{0}';ErrorMessage='{1}'", _currentDirectory, ex.Message));
				throw new FtpException("An error occurred while executing PutFileUnique() on the remote FTP destination.", base.LastResponse, ex);
			}

			WriteToLog(String.Format("Action='PutFileUnique';Action='TransferSuccess';CurrentDirectory='{0}'", _currentDirectory));

			//  TODO: return the name of the file created on the server
		}

		/// <summary>
		/// Retrieves a remote file from the FTP server and writes the data to a local file
		/// specfied in the localPath parameter.  If the local file already exists a System.IO.IOException is thrown.
		/// </summary>
		/// <remarks>
		/// To retrieve a remote file that you need to overwrite an existing file with or append to an existing file
		/// see the method GetFile(string, string, FileAction).
		/// </remarks>
		/// <param name="remotePath">A path of the remote file.</param>
		/// <param name="localPath">A fully qualified local path to a file on the local machine.</param>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>
		public virtual void GetFile(string remotePath, string localPath) {
			GetFile(remotePath, localPath, FileAction.CreateNew);
		}

		/// <summary>
		/// Retrieves a remote file from the FTP server and writes the data to a local file
		/// specfied in the localPath parameter.
		/// </summary>
		/// <remarks>
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to get the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <param name="remotePath">A path and/or file name to the remote file.</param>
		/// <param name="localPath">A fully qualified local path to a file on the local machine.</param>
		/// <param name="action">The type of action to take.</param>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>
		public virtual void GetFile(string remotePath, string localPath, FileAction action) {
			if (remotePath == null)
				throw new ArgumentNullException("remotePath");

			if (remotePath.Length == 0)
				throw new ArgumentException("must contain a value", "remotePath");

			if (localPath == null)
				throw new ArgumentNullException("localPath");

			if (localPath.Length == 0)
				throw new ArgumentException("must contain a value", "localPath");

			if (action == FileAction.None)
				throw new ArgumentOutOfRangeException("action", "must contain a value other than 'Unknown'");

			localPath = CorrectLocalPath(localPath);

			WriteToLog(String.Format("Action='GetFile';Status='TransferBegin';LocalPath='{0}';RemotePath='{1}';FileAction='{1}'", localPath, remotePath, action.ToString()));

			FtpRequest request = new FtpRequest(base.CharacterEncoding, FtpCmd.Retr, remotePath);

			try {
				switch (action) {
					case FileAction.CreateNew:
						// create a file stream to stream the file locally to disk that only creates the file if it does not already exist
						using (Stream localFile = File.Open(localPath, FileMode.CreateNew)) {
							base.TransferData(TransferDirection.ToClient, request, localFile);
						}
						break;

					case FileAction.Create:
						// create a file stream to stream the file locally to disk
						using (Stream localFile = File.Open(localPath, FileMode.Create)) {
							base.TransferData(TransferDirection.ToClient, request, localFile);
						}
						break;
					case FileAction.CreateOrAppend:
						// open the local file
						using (Stream localFile = File.Open(localPath, FileMode.OpenOrCreate)) {
							// set the file position to the end so that any new data will be appended                        
							localFile.Position = localFile.Length;
							base.TransferData(TransferDirection.ToClient, request, localFile);
						}
						break;
					case FileAction.Resume:
						using (Stream localFile = File.Open(localPath, FileMode.Open)) {
							//  get the size of the file already on the server (in bytes)
							long remoteSize = GetFileSize(remotePath);

							// if the files are the same size then there is nothing to transfer
							if (localFile.Length == remoteSize)
								return;

							base.TransferData(TransferDirection.ToClient, request, localFile, localFile.Length - 1);
						}
						break;
					case FileAction.ResumeOrCreate:
						if (File.Exists(localPath) && (new FileInfo(localPath)).Length > 0)
							GetFile(remotePath, localPath, FileAction.Resume);
						else
							GetFile(remotePath, localPath, FileAction.Create);
						break;
				}
			} catch (Exception ex) {
				WriteToLog(String.Format("Action='GetFile';Status='TransferError';LocalPath='{0}';RemotePath='{1}';FileAction='{1}';ErrorMessage='{2}", localPath, remotePath, action.ToString(), ex.Message));
				throw new FtpException(String.Format("An unexpected exception occurred while retrieving file '{0}'.", remotePath), base.LastResponse, ex);
			}

			WriteToLog(String.Format("Action='GetFile';Status='TransferSuccess';LocalPath='{0}';RemotePath='{1}';FileAction='{1}'", localPath, remotePath, action.ToString()));
		}

		/// <summary>
		/// Retrieves a remote file from the FTP server and writes the data to a local stream object
		/// specfied in the outStream parameter.
		/// </summary> 
		/// <param name="remotePath">A path and/or file name to the remote file.</param>
		/// <param name="outStream">An output stream object used to stream the remote file to the local machine.</param>
		/// <param name="restart">A true/false value to indicate if the file download needs to be restarted due to a previous partial download.</param>
		/// <remarks>
		/// If the remote path is a file name then the file is downloaded from the FTP server current working directory.  Otherwise a fully qualified
		/// path for the remote file may be specified.  The output stream must be writeable and can be any stream object.  Finally, the restart parameter
		/// is used to send a restart command to the FTP server.  The FTP server is instructed to restart the download process at the last position of
		/// of the output stream.  Not all FTP servers support the restart command.  If the FTP server does not support the restart (REST) command,
		/// an FtpException error is thrown.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to get the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>
		public virtual void GetFile(string remotePath, Stream outStream, bool restart) {
			if (remotePath == null)
				throw new ArgumentNullException("remotePath");

			if (remotePath.Length == 0)
				throw new ArgumentException("must contain a value", "remotePath");

			if (outStream == null)
				throw new ArgumentNullException("outStream");

			if (outStream.CanWrite == false)
				throw new ArgumentException("must be writable.  The CanWrite property must return the value 'true'.", "outStream");

			WriteToLog(String.Format("Action='GetFile';Status='TransferBegin';RemotePath='{0}';", remotePath));

			FtpRequest request = new FtpRequest(base.CharacterEncoding, FtpCmd.Retr, remotePath);

			if (restart) {
				//  get the size of the file already on the server (in bytes)
				long remoteSize = GetFileSize(remotePath);

				// if the files are the same size then there is nothing to transfer
				if (outStream.Length == remoteSize)
					return;

				base.TransferData(TransferDirection.ToClient, request, outStream, outStream.Length - 1);
			} else {
				base.TransferData(TransferDirection.ToClient, request, outStream);
			}
			WriteToLog(String.Format("Action='GetFile';Status='TransferSuccess';RemotePath='{0}';", remotePath));

		}

		/// <summary>
		/// Tests to see if a file or directory exists on the remote server.  The current working directory must be the
		/// parent or root directory of the file or directory whose existence is being tested.  For best results, 
		/// call this method from the root working directory ("/").
		/// </summary>
		/// <param name="path">The full path to the remote file or directory relative to the current working directory, or the filename 
		/// or directory in the current working directory.</param>
		/// <returns>Boolean value indicating if file exists or not.</returns>
		/// <remarks>This method will execute a change working directory (CWD) command prior to testing to see if the  
		/// file or direcotry exists.  The original working directory will be changed back to the original value
		/// after this method has completed.  This method may not work on systems where the directory listing is not being
		/// parsed correctly.  If the method call GetDirList() does not work properly with your FTP server, this method may not
		/// produce reliable results.  This method will also not produce reliable results if the directory or file is hidden on the
		/// remote FTP server.</remarks>
		/// <seealso cref="GetDirList()"/>
		public virtual bool Exists(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			// replace the windows style directory delimiter with a unix style delimiter
			int chgDirCnt = 0;
			bool found = false;
			bool errorChgDir = false;
			path = path.Replace("\\", "/");

			string[] dirs = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			string filename = "";
			if (dirs.Length < 2)
				filename = path;
			else
				filename = dirs[dirs.Length - 1];

			try {
				// if the path contains more than just the filename then 
				// we must change the directory to where the file is located
				if (dirs.Length > 1) {
					for (int i = 0; i < dirs.Length - 1; i++) {
						ChangeDirectory(dirs[i]);
						chgDirCnt++;
					}
				}
			} catch (FtpException) {
				errorChgDir = true;
			}

			try {
				if (!errorChgDir)
					found = GetDirList().ContainsName(filename);
			} catch (FtpException) { }

			try {
				// change directory back up to the original directory
				// this is a very reliable method to change directories on all FTP servers
				for (int j = 0; j < chgDirCnt; j++)
					this.ChangeDirectoryUp();
			} catch (FtpException) { }

			return found;
		}


		/// <summary>
		/// Retrieves a file name listing of the current working directory from the 
		/// remote FTP server using the NLST command.
		/// </summary>
		/// <returns>A string containing the file listing from the current working directory.</returns>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		public virtual string GetNameList() {
			return base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.Nlst));
		}

		/// <summary>
		/// Retrieves a file name listing of the current working directory from the 
		/// remote FTP server using the NLST command.
		/// </summary>
		/// <param name="path">The path to a directory on the remote FTP server.</param>
		/// <returns>A string containing the file listing from the current working directory.</returns>
		/// <remarks>
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the parent directory you wish to get the name list using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		public virtual string GetNameList(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			return base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.Nlst, path));
		}


		/// <summary>
		/// Retrieves a directory listing of the current working directory from the 
		/// remote FTP server using the LIST command.
		/// </summary>
		/// <returns>A string containing the directory listing of files from the current working directory.</returns>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		/// <seealso cref="GetNameList(string)"/>
		public virtual string GetDirListAsText() {
			return base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.List, "-al"));
		}

		/// <summary>
		/// Retrieves a directory listing of the current working directory from the 
		/// remote FTP server using the LIST command.
		/// </summary>
		/// <param name="path">The path to a directory on the remote FTP server.</param>
		/// <returns>A string containing the directory listing of files from the current working directory.</returns>
		/// <remarks>
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the parent directory you wish to get the name list using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync()"/>
		/// <seealso cref="GetNameList()"/>
		/// <seealso cref="GetNameList(string)"/>
		public virtual string GetDirListAsText(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			return base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.List, "-al", path));
		}

		/// <summary>
		/// Retrieves a list of the files from current working directory on the remote FTP 
		/// server using the LIST command.  
		/// </summary>
		/// <returns>FtpItemList collection object.</returns>
		/// <remarks>
		/// This method returns a FtpItemList collection of FtpItem objects.
		/// </remarks>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		/// <seealso cref="GetNameList(string)"/>        
		public virtual FtpItemCollection GetDirList() {
			var items = new FtpItemCollection(_currentDirectory, base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.List, "-al")), _itemParser);
			items.SetTimeOffset(ServerTimeOffset);
			return items;
		}

		/// <summary>
		/// Retrieves a list of the files from a specified path on the remote FTP 
		/// server using the LIST command. 
		/// </summary>
		/// <param name="path">The path to a directory on the remote FTP server.</param>
		/// <returns>FtpFileCollection collection object.</returns>
		/// <remarks>
		/// This method returns a FtpFileCollection object containing a collection of 
		/// FtpItem objects.  Some FTP server implementations will not accept a full path to a resource.  On those
		/// systems it is best to change the working directory using the ChangeDirectoryMultiPath(string) method and then call
		/// the method GetDirList().
		/// </remarks>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		/// <seealso cref="GetNameList(string)"/>        
		public virtual FtpItemCollection GetDirList(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			var items = new FtpItemCollection(path, base.TransferText(new FtpRequest(base.CharacterEncoding, FtpCmd.List, "-al", path)), _itemParser);
			items.SetTimeOffset(ServerTimeOffset);
			return items;
		}

		/// <summary>
		/// Deeply retrieves a list of all files and all sub directories from a specified path on the remote FTP 
		/// server using the LIST command. 
		/// </summary>
		/// <param name="path">The path to a directory on the remote FTP server.</param>
		/// <returns>FtpFileCollection collection object.</returns>
		/// <remarks>
		/// This method returns a FtpFileCollection object containing a collection of 
		/// FtpItem objects.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the parent directory you wish to get the directory list using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetNameList(string)"/>
		public virtual FtpItemCollection GetDirListDeep(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			FtpItemCollection deepCol = new FtpItemCollection();
			ParseDirListDeep(path, deepCol);
			deepCol.SetTimeOffset(ServerTimeOffset);
			return deepCol;
		}

		/// <summary>
		/// Renames a file or directory on the remote FTP server.
		/// </summary>
		/// <param name="name">The name or absolute path of the file or directory you want to rename.</param>
		/// <param name="newName">The new name or absolute path of the file or directory.</param>
		/// <seealso cref="SetDateTime"/>
		public virtual void Rename(string name, string newName) {
			if (name == null)
				throw new ArgumentNullException("name", "must have a value");

			if (name.Length == 0)
				throw new ArgumentException("must have a value", "name");

			if (newName == null)
				throw new ArgumentNullException("newName", "must have a value");

			if (newName.Length == 0)
				throw new ArgumentException("must have a value", "newName");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Rnfr, name));
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Rnto, newName));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("The FTP destination was unable to rename the file or directory '{0}' to the new name '{1}'.", name, newName), base.LastResponse, fex);
			}

		}

		/// <summary>
		/// Send a raw FTP command to the server.
		/// </summary>
		/// <param name="command">A string containing a valid FTP command value such as SYST.</param>
		/// <returns>The raw textual response from the server.</returns>
		/// <remarks>
		/// This is an advanced feature of the FtpClient class that allows for any custom or specialized
		/// FTP command to be sent to the FTP server.  Some FTP server support custom commands outside of
		/// the standard FTP command list.  The following commands are not supported: PASV, RETR, STOR, and STRU.
		/// </remarks>
		/// <example>
		/// <code>
		/// FtpClient ftp = new FtpClient("ftp.microsoft.com");
		/// ftp.Open("anonymous", "myemail@server.com");
		/// string response = ftp.Quote("SYST");
		/// System.Diagnostics.Debug.WriteLine(response);
		/// ftp.Close();
		/// </code>
		/// </example>
		public virtual string Quote(string command) {
			if (command == null)
				throw new ArgumentNullException("command");

			if (command.Length < 3) {
				throw new ArgumentException(String.Format("Invalid command '{0}'.", command), "command");
			}

			char[] separator = { ' ' };
			string[] values = command.Split(separator);

			// extract just the code value
			string code;
			if (values.Length == 0) {
				code = command;
			} else {
				code = values[0];
			}

			// extract the arguments
			string args = string.Empty;
			if (command.Length > code.Length) {
				args = command.Replace(code, "").TrimStart();
			}

			FtpCmd ftpCmd = FtpCmd.Unknown;
			try {
				// try to parse out the command if we can
				ftpCmd = (FtpCmd)Enum.Parse(typeof(FtpCmd), code, true);
			} catch { }

			if (ftpCmd == FtpCmd.Pasv || ftpCmd == FtpCmd.Retr || ftpCmd == FtpCmd.Stor || ftpCmd == FtpCmd.Stou || ftpCmd == FtpCmd.Erpt || ftpCmd == FtpCmd.Epsv)
				throw new ArgumentException(String.Format("Command '{0}' not supported by Quote() method.", code), "command");

			if (ftpCmd == FtpCmd.List || ftpCmd == FtpCmd.Nlst)
				return base.TransferText(new FtpRequest(base.CharacterEncoding, ftpCmd, args));


			if (ftpCmd == FtpCmd.Unknown)
				base.SendRequest(new FtpRequest(base.CharacterEncoding, ftpCmd, command));
			else
				base.SendRequest(new FtpRequest(base.CharacterEncoding, ftpCmd, args));

			return base.LastResponseList.GetRawText();
		}

		/// <summary>
		/// Sends a NOOP or no operation command to the FTP server.  This can be used to prevent some servers from logging out the
		/// interactive session during file transfer process.
		/// </summary>
		public virtual void NoOperation() {
			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Noop));
			} catch (FtpException fex) {
				throw new FtpException("An error occurred while issuing the No Operation command (NOOP).", base.LastResponse, fex);
			}
		}

		/// <summary>
		/// Issues a site specific change file mode (CHMOD) command to the server.  Not all servers implement this command.
		/// </summary>
		/// <param name="path">The path to the file or directory you want to change the mode on.</param>
		/// <param name="octalValue">The CHMOD octal value.</param>
		/// <remarks>
		/// Common CHMOD values used on web servers.
		/// 
		///       Value 	User 	Group 	Other
		///         755 	rwx 	r-x 	r-x
		///         744 	rwx 	r--	    r--
		///         766 	rwx 	rw- 	rw-
		///         777 	rwx 	rwx 	rwx
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory containing the file or directory you wish to change the mode on by using with the 
		/// ChangeDirectory() or ChangeDirectoryMultiPath() method.
		/// </remarks>
		/// <seealso cref="SetDateTime"/>
		/// <seealso cref="Rename"/>
		public virtual void ChangeMode(string path, int octalValue) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			try {
				base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Site, "CHMOD", octalValue.ToString(), path));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("Unable to the change file mode for file {0}.  Reason: {1}", path, base.LastResponse.Text), base.LastResponse, fex);
			}

			if (base.LastResponse.Code == FtpResponseCode.CommandNotImplementedSuperfluousAtThisSite)
				throw new FtpException(String.Format("Unable to the change file mode for file {0}.  Reason: {1}", path, base.LastResponse.Text), base.LastResponse);
		}

		/// <summary>
		/// Issue a SITE command to the FTP server for site specific implementation commands.
		/// </summary>
		/// <param name="argument">One or more command arguments</param>
		/// <remarks>
		/// For example, the CHMOD command is issued as a SITE command.
		/// </remarks>
		public virtual void Site(string argument) {
			if (argument == null)
				throw new ArgumentNullException("argument", "must have a value");

			if (argument.Length == 0)
				throw new ArgumentException("must have a value", "argument");

			base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Site, argument));
		}

		/// <summary>
		/// Uploads a local file specified in the path parameter to the remote FTP server.   
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <param name="remotePath">Filename or full path to file on the remote FTP server.</param>
		/// <param name="action">The type of put action taken.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server.  The remotePath
		/// parameter is used to specify the path and file name used to store the file on the remote server.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to put the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>    
		public virtual void PutFile(string localPath, string remotePath, FileAction action) {
			using (FileStream fileStream = File.OpenRead(localPath)) {
				PutFile(fileStream, remotePath, action);
			}
		}

		/// <summary>
		/// Uploads a local file specified in the path parameter to the remote FTP server.   
		/// An FtpException is thrown if the file already exists.
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <param name="remotePath">Filename or full path to file on the remote FTP server.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server.  The remotePath
		/// parameter is used to specify the path and file name used to store the file on the remote server.
		/// To overwrite an existing file see the method PutFile(string, string, FileAction) and specify the 
		/// FileAction Create.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to put the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>            
		public virtual void PutFile(string localPath, string remotePath) {
			using (FileStream fileStream = File.OpenRead(localPath)) {
				PutFile(fileStream, remotePath, FileAction.CreateNew);
			}
		}

		/// <summary>
		/// Uploads a local file specified in the path parameter to the remote FTP server.   
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <param name="action">The type of put action taken.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server. 
		/// </remarks>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>            
		public virtual void PutFile(string localPath, FileAction action) {
			using (FileStream fileStream = File.OpenRead(localPath)) {
				PutFile(fileStream, ExtractPathItemName(localPath), action);
			}
		}

		/// <summary>
		/// Uploads a local file specified in the path parameter to the remote FTP server.   
		/// An FtpException is thrown if the file already exists.
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server. 
		/// </remarks>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>    
		public virtual void PutFile(string localPath) {
			using (FileStream fileStream = File.OpenRead(localPath)) {
				PutFile(fileStream, ExtractPathItemName(localPath), FileAction.CreateNew);
			}
		}

		/// <summary>
		/// Uploads stream data specified in the inputStream parameter to the remote FTP server.   
		/// </summary>
		/// <param name="inputStream">Any open stream object on the local client machine.</param>
		/// <param name="remotePath">Filename or path and filename of the file stored on the remote FTP server.</param>
		/// <param name="action">The type of put action taken.</param>
		/// <remarks>
		/// The stream is uploaded to the current working directory on the remote server.  The remotePath
		/// parameter is used to specify the path and file name used to store the file on the remote server.
		/// Note that some FTP servers will not accept a full path.  On those systems you must navigate to
		/// the directory you wish to put the file using with the ChangeDirectory() or ChangeDirectoryMultiPath()
		/// method.
		/// </remarks>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>        
		public virtual void PutFile(Stream inputStream, string remotePath, FileAction action) {
			if (inputStream == null)
				throw new ArgumentNullException("inputStream");

			if (!inputStream.CanRead)
				throw new ArgumentException("must be readable", "inputStream");

			if (remotePath == null)
				throw new ArgumentNullException("remotePath");

			if (remotePath.Length == 0)
				throw new ArgumentException("must contain a value", "remotePath");

			if (action == FileAction.None)
				throw new ArgumentOutOfRangeException("action", "must contain a value other than 'Unknown'");

			WriteToLog(String.Format("Action='PutFile';Status='TransferBegin';RemotePath='{0}';FileAction='{1}'", remotePath, action.ToString()));

			try {
				switch (action) {
					case FileAction.CreateOrAppend:
						base.TransferData(TransferDirection.ToServer, new FtpRequest(base.CharacterEncoding, FtpCmd.Appe, remotePath), inputStream);
						break;
					case FileAction.CreateNew:
						if (Exists(remotePath)) {
							throw new FtpException("Cannot overwrite existing file when action FileAction.CreateNew is specified.");
						}
						base.TransferData(TransferDirection.ToServer, new FtpRequest(base.CharacterEncoding, FtpCmd.Stor, remotePath), inputStream);
						break;
					case FileAction.Create:
						base.TransferData(TransferDirection.ToServer, new FtpRequest(base.CharacterEncoding, FtpCmd.Stor, remotePath), inputStream);
						break;
					case FileAction.Resume:
						//  get the size of the file already on the server (in bytes)
						long remoteSize = GetFileSize(remotePath);

						//  if the files are the same size then there is nothing to transfer
						if (remoteSize == inputStream.Length)
							return;

						//  transfer file to the server
						base.TransferData(TransferDirection.ToServer, new FtpRequest(base.CharacterEncoding, FtpCmd.Stor, remotePath), inputStream, remoteSize);
						break;
					case FileAction.ResumeOrCreate:
						if (Exists(remotePath))
							PutFile(inputStream, remotePath, FileAction.Resume);
						else
							PutFile(inputStream, remotePath, FileAction.Create);
						break;
				}
			} catch (FtpException fex) {
				WriteToLog(String.Format("Action='PutFile';Status='TransferError';RemotePath='{0}';FileAction='{1}';ErrorMessage='{2}'", remotePath, action.ToString(), fex.Message));
				throw new FtpDataTransferException(String.Format("An error occurred while putting fileName '{0}'.", remotePath), base.LastResponse, fex);
			}

			WriteToLog(String.Format("Action='PutFile';Status='TransferSuccess';RemotePath='{0}';FileAction='{1}'", remotePath, action.ToString()));
		}

		/// <summary>
		/// File Exchange Protocol (FXP) allows server-to-server transfer which can greatly speed up file transfers.
		/// </summary>
		/// <param name="fileName">The name of the file to transfer.</param>
		/// <param name="destination">The destination FTP server which must be supplied as an open and connected FtpClient object.</param>
		/// <remarks>
		/// Both servers must support and have FXP enabled before you can transfer files between two remote servers using FXP.  One FTP server must support PASV mode and the other server must allow PORT commands from a foreign address.  Finally, firewall settings may interfer with the ability of one server to access the other server.
		/// Starksoft FtpClient will coordinate the FTP negoitaion and necessary PORT and PASV transfer commands.
		/// </remarks>
		/// <seealso cref="FxpTransferTimeout"/>
		/// <seealso cref="FxpCopyAsync"/> 
		public virtual void FxpCopy(string fileName, FtpClient destination) {

			if (this.IsConnected == false)
				throw new FtpException("The connection must be open before a transfer between servers can be intitiated.");

			if (destination == null)
				throw new ArgumentNullException("destination");

			if (destination.IsConnected == false)
				throw new FtpException("The destination object must be open and connected before a transfer between servers can be intitiated.");

			if (fileName == null)
				throw new ArgumentNullException("fileName");

			if (fileName.Length == 0)
				throw new ArgumentException("must have a value", "fileName");


			//  send command to destination FTP server to get passive port to be used from the source FTP server
			try {
				destination.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Pasv));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred when trying to set up the passive connection on '{1}' for a destination to destination copy between '{0}' and '{1}'.", this.Host, destination.Host), base.LastResponse, fex);
			}

			//  get the begin and end positions to extract data from the response string
			int startIdx = destination.LastResponse.Text.IndexOf("(") + 1;
			int endIdx = destination.LastResponse.Text.IndexOf(")");
			string dataPortInfo = destination.LastResponse.Text.Substring(startIdx, endIdx - startIdx);

			//  send a command to the source server instructing it to connect to
			//  the local ip address and port that the destination server will be bound to
			try {
				this.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Port, dataPortInfo));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("Command instructing '{0}' to open connection failed.", this.Host), base.LastResponse, fex);
			}

			// send command to tell the source server to retrieve the file from the destination server
			try {
				this.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Retr, fileName));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred transfering on a server to server copy between '{0}' and '{1}'.", this.Host, destination.Host), base.LastResponse, fex);
			}

			// send command to tell the destination to store the file
			try {
				destination.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Stor, fileName));
			} catch (FtpException fex) {
				throw new FtpException(String.Format("An error occurred transfering on a server to server copy between '{0}' and '{1}'.", this.Host, destination.Host), base.LastResponse, fex);
			}

			// wait until we get a file completed response back from the destination server and the source server
			destination.WaitForHappyCodes(this.FxpTransferTimeout, FtpResponseCode.RequestedFileActionOkayAndCompleted, FtpResponseCode.ClosingDataConnection);
			this.WaitForHappyCodes(this.FxpTransferTimeout, FtpResponseCode.RequestedFileActionOkayAndCompleted, FtpResponseCode.ClosingDataConnection);
		}

		#endregion

		#region Private Methods

		private void ParseDirListDeep(string path, FtpItemCollection deepCol) {
			FtpItemCollection itemCol = GetDirList(path);
			deepCol.Merge(itemCol);

			foreach (FtpItem item in itemCol) {
				// if the this call is being completed asynchronously and the user requests a cancellation
				// then stop processing the items and return
				if (base.AsyncWorker != null && base.AsyncWorker.CancellationPending)
					return;

				// if the item is of type Directory then parse the directory list recursively
				if (item.ItemType == FtpItemType.Directory)
					ParseDirListDeep(item.FullPath, deepCol);
			}
		}

		private string CorrectRemotePath(string path) {
			if (path.StartsWith("/")) return RootDirectory + path;
			else return path;
		}

		private string CorrectLocalPath(string path) {
			if (path == null)
				throw new ArgumentNullException("path");

			if (path.Length == 0)
				throw new ArgumentException("must have a value", "path");

			string fileName = ExtractPathItemName(path);
			string pathOnly = path.Substring(0, path.Length - fileName.Length - 1);

			// if the pathOnly portion contains the root node then we need to add the 
			// a directory slash back otherwise the final combined path will be something
			// like c:myfile.txt and this will result
			if (pathOnly.EndsWith(":") && pathOnly.IndexOf("\\") == -1) {
				pathOnly += "\\";
			}

			char[] invalidPath = Path.GetInvalidPathChars();
			if (path.IndexOfAny(invalidPath) != -1) {
				for (int i = 0; i < invalidPath.Length; i++) {
					if (pathOnly.IndexOf(invalidPath[i]) != -1)
						pathOnly = pathOnly.Replace(invalidPath[i], '_');
				}
			}

			char[] invalidFile = Path.GetInvalidFileNameChars();
			if (fileName.IndexOfAny(invalidFile) != -1) {
				for (int i = 0; i < invalidFile.Length; i++) {
					if (fileName.IndexOf(invalidFile[i]) != -1)
						fileName = fileName.Replace(invalidFile[i], '_');
				}
			}

			return Path.Combine(pathOnly, fileName);

		}

		private void SetFileTransferType() {
			switch (_fileTransferType) {
				case TransferType.Binary:
					base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Type, "I"));
					break;
				case TransferType.Ascii:
					base.SendRequest(new FtpRequest(base.CharacterEncoding, FtpCmd.Type, "A"));
					break;
			}
		}



		private string ExtractPathItemName(string path) {
			if (path.IndexOf("\\") != -1)
				return path.Substring(path.LastIndexOf("\\") + 1);
			else if (path.IndexOf("/") != -1)
				return path.Substring(path.LastIndexOf("/") + 1);
			else if (path.Length > 0)
				return path;
			else
				throw new FtpException(String.Format(CultureInfo.InvariantCulture, "Item name not found in path {0}.", path));
		}


		private void WriteToLog(string message) {
			if (!_isLoggingOn)
				return;

			string line0 = String.Format("[{0}] [{1}] [{2}] {3}", DateTime.Now.ToString("G"), base.Host, base.Port.ToString(), message);
			string line = line0 + "\r\n";
			if (_log != null) {
				byte[] buffer = base.CharacterEncoding.GetBytes(line);
				_log.Write(buffer, 0, buffer.Length);
			}
			Job.Log.Text(line0);
		}

		#endregion

		#region  Asynchronous Methods and Events

		private Exception _asyncException;

		////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Event handler for GetDirListAsync method.
		/// </summary>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		public event EventHandler<GetDirListAsyncCompletedEventArgs> GetDirListAsyncCompleted;

		/// <summary>
		/// Asynchronously retrieves a list of the files from current working directory on the remote FTP 
		/// server using the LIST command.  
		/// </summary>
		/// <remarks>
		/// This method returns a FtpItemList collection of FtpItem objects through the GetDirListAsyncCompleted event.
		/// </remarks>
		/// <seealso cref="GetDirListAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetNameList(string)"/>
		public void GetDirListAsync() {
			GetDirListAsync(string.Empty);
		}

		/// <summary>
		/// Asynchronously retrieves a list of the files from a specified path on the remote FTP 
		/// server using the LIST command. 
		/// </summary>
		/// <param name="path">The path to a directory on the remote FTP server.</param>
		/// <remarks>This method returns a FtpFileCollection object containing a collection of 
		/// FtpItem objects.  The FtpFileCollection is returned though the GetDirListAsyncCompleted event.</remarks>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirListDeepAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetNameList(string)"/>        
		public void GetDirListAsync(string path) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(GetDirListAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetDirListAsync_RunWorkerCompleted);
			base.AsyncWorker.RunWorkerAsync(path);
		}

		private void GetDirListAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				string path = (string)e.Argument;
				e.Result = GetDirList(path);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		private void GetDirListAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (GetDirListAsyncCompleted != null)
				GetDirListAsyncCompleted(this, new GetDirListAsyncCompletedEventArgs(_asyncException, base.IsAsyncCanceled, (FtpItemCollection)e.Result));
			_asyncException = null;
		}

		////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Event handler for GetDirListDeepAsync method.
		/// </summary>
		public event EventHandler<GetDirListDeepAsyncCompletedEventArgs> GetDirListDeepAsyncCompleted;

		/// <summary>
		/// Asynchronous deep retrieval of a list of all files and all sub directories from the current path on the remote FTP 
		/// server using the LIST command. 
		/// </summary>
		/// <remarks>This method returns a FtpFileCollection object containing a collection of FtpItem objects through the GetDirListDeepAsyncCompleted event.</remarks>
		/// <seealso cref="GetDirListDeepAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		/// <seealso cref="GetNameList(string)"/>
		public void GetDirListDeepAsync() {
			GetDirListDeepAsync(GetWorkingDirectory());
		}

		/// <summary>
		/// Asynchronous deep retrieval of a list of all files and all sub directories from a specified path on the remote FTP 
		/// server using the LIST command. 
		/// </summary>
		/// <param name="path">The path to a directory on the remote FTP server.</param>
		/// <remarks>This method returns a FtpFileCollection object containing a collection of 
		/// FtpItem objects the GetDirListDeepAsyncCompleted event.</remarks>
		/// <seealso cref="GetDirListDeepAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="GetDirListDeep"/>
		/// <seealso cref="GetDirList(string)"/>
		/// <seealso cref="GetDirListAsync(string)"/>
		/// <seealso cref="GetDirListAsText(string)"/>
		public void GetDirListDeepAsync(string path) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(GetDirListDeepAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetDirListDeepAsync_RunWorkerCompleted);
			base.AsyncWorker.RunWorkerAsync(path);
		}

		private void GetDirListDeepAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				string path = (string)e.Argument;
				e.Result = GetDirList(path);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		private void GetDirListDeepAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (GetDirListDeepAsyncCompleted != null)
				GetDirListDeepAsyncCompleted(this, new GetDirListDeepAsyncCompletedEventArgs(_asyncException, base.IsAsyncCanceled, (FtpItemCollection)e.Result));
			_asyncException = null;
		}


		////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Event that fires when the GetFileAsync method is invoked.
		/// </summary>
		public event EventHandler<GetFileAsyncCompletedEventArgs> GetFileAsyncCompleted;

		/// <summary>
		/// Asynchronously retrieves a remote file from the FTP server and writes the data to a local file
		/// specfied in the localPath parameter.
		/// </summary>
		/// <param name="remotePath">A path and/or file name to the remote file.</param>
		/// <param name="localPath">A fully qualified local path to a file on the local machine.</param>
		/// <param name="action">The type of action to take.</param>
		/// <seealso cref="GetFileAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="PutFile(string)"/>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>
		public void GetFileAsync(string remotePath, string localPath, FileAction action) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(GetFileAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetFileAsync_RunWorkerCompleted);
			Object[] args = new Object[3];
			args[0] = remotePath;
			args[1] = localPath;
			args[2] = action;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void GetFileAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				GetFile((string)args[0], (string)args[1], (FileAction)args[2]);
			} catch (Exception ex) {
				_asyncException = ex;
			}

		}

		private void GetFileAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (GetFileAsyncCompleted != null)
				GetFileAsyncCompleted(this, new GetFileAsyncCompletedEventArgs(_asyncException, base.IsAsyncCanceled));
			_asyncException = null;
		}

		/// <summary>
		/// Asynchronously retrieves a remote file from the FTP server and writes the data to a local stream object
		/// specfied in the outStream parameter.
		/// </summary> 
		/// <param name="remotePath">A path and/or file name to the remote file.</param>
		/// <param name="outStream">An output stream object used to stream the remote file to the local machine.</param>
		/// <param name="restart">A true/false value to indicate if the file download needs to be restarted due to a previous partial download.</param>
		/// <remarks>
		/// If the remote path is a file name then the file is downloaded from the FTP server current working directory.  Otherwise a fully qualified
		/// path for the remote file may be specified.  The output stream must be writeable and can be any stream object.  Finally, the restart parameter
		/// is used to send a restart command to the FTP server.  The FTP server is instructed to restart the download process at the last position of
		/// of the output stream.  Not all FTP servers support the restart command.  If the FTP server does not support the restart (REST) command,
		/// an FtpException error is thrown.
		/// </remarks>        
		/// <seealso cref="GetFileAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="GetFile(string, string)"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>
		public void GetFileAsync(string remotePath, Stream outStream, bool restart) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(GetFileStreamAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetFileAsync_RunWorkerCompleted);
			Object[] args = new Object[3];
			args[0] = remotePath;
			args[1] = outStream;
			args[2] = restart;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void GetFileStreamAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				GetFile((string)args[0], (Stream)args[1], (bool)args[2]);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Asynchronous event for PutFileAsync method.
		/// </summary>
		/// <seealso cref="PutFileAsync(string, string, FileAction)"/>
		public event EventHandler<PutFileAsyncCompletedEventArgs> PutFileAsyncCompleted;

		/// <summary>
		/// Asynchronously uploads a local file specified in the path parameter to the remote FTP server.   
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <param name="remotePath">Filename or full path to file on the remote FTP server.</param>
		/// <param name="action">The type of put action taken.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server.  The remotePath
		/// parameter is used to specify the path and file name used to store the file on the remote server.
		/// </remarks>
		/// <seealso cref="PutFileAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>  
		public void PutFileAsync(string localPath, string remotePath, FileAction action) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(PutFileAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PutFileAsync_RunWorkerCompleted);
			Object[] args = new Object[3];
			args[0] = localPath;
			args[1] = remotePath;
			args[2] = action;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void PutFileAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				PutFile((string)args[0], (string)args[1], (FileAction)args[2]);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		private void PutFileAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (PutFileAsyncCompleted != null)
				PutFileAsyncCompleted(this, new PutFileAsyncCompletedEventArgs(_asyncException, base.IsAsyncCanceled));
			_asyncException = null;
		}

		/// <summary>
		/// Asynchronously uploads stream data specified in the inputStream parameter to the remote FTP server.   
		/// </summary>
		/// <param name="inputStream">Any open stream object on the local client machine.</param>
		/// <param name="remotePath">Filename or path and filename of the file stored on the remote FTP server.</param>
		/// <param name="action">The type of put action taken.</param>
		/// <remarks>
		/// The stream is uploaded to the current working directory on the remote server.  The remotePath
		/// parameter is used to specify the path and file name used to store the file on the remote server.
		/// </remarks>
		/// <seealso cref="PutFileAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>    
		public void PutFileAsync(Stream inputStream, string remotePath, FileAction action) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(PutFileStreamAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PutFileAsync_RunWorkerCompleted);
			Object[] args = new Object[3];
			args[0] = inputStream;
			args[1] = remotePath;
			args[2] = action;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void PutFileStreamAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				PutFile((Stream)args[0], (string)args[1], (FileAction)args[2]);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		/// <summary>
		/// Asynchronously uploads a local file specified in the path parameter to the remote FTP server.   
		/// </summary>
		/// <param name="localPath">Path to a file on the local machine.</param>
		/// <param name="action">The type of put action taken.</param>
		/// <remarks>
		/// The file is uploaded to the current working directory on the remote server. 
		/// </remarks>
		/// <seealso cref="PutFileAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="PutFile(string, string, FileAction)"/>
		/// <seealso cref="PutFileUnique(string)"/>
		/// <seealso cref="GetFile(string, string, FileAction)"/>
		/// <seealso cref="GetFileAsync(string, string, FileAction)"/>
		/// <seealso cref="MoveFile"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FxpCopyAsync"/>    
		public void PutFileAsync(string localPath, FileAction action) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(PutFileLocalAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PutFileAsync_RunWorkerCompleted);
			Object[] args = new Object[2];
			args[0] = localPath;
			args[1] = action;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void PutFileLocalAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				PutFile((string)args[0], (FileAction)args[1]);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Event handler for OpenAsync method.
		/// </summary>
		public event EventHandler<OpenAsyncCompletedEventArgs> OpenAsyncCompleted;

		/// <summary>
		/// Asynchronously opens a connection to the remote FTP server and log in with user name and password credentials.
		/// </summary>
		/// <param name="user">User name.  Many public ftp allow for this value to 'anonymous'.</param>
		/// <param name="password">Password.  Anonymous public ftp servers generally require a valid email address for a password.</param>
		/// <remarks>Use the Close() method to log off and close the connection to the FTP server.</remarks>
		/// <seealso cref="OpenAsyncCompleted"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		/// <seealso cref="Open"/>
		/// <seealso cref="Reopen"/>
		/// <seealso cref="Close"/>
		public void OpenAsync(string user, string password) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(OpenAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OpenAsync_RunWorkerCompleted);
			Object[] args = new Object[2];
			args[0] = user;
			args[1] = password;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void OpenAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				Open((string)args[0], (string)args[1]);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		private void OpenAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (OpenAsyncCompleted != null)
				OpenAsyncCompleted(this, new OpenAsyncCompletedEventArgs(_asyncException, base.IsAsyncCanceled));
			_asyncException = null;
		}


		////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Asynchronous event for FxpCopyAsync method.
		/// </summary>
		public event EventHandler<FxpCopyAsyncCompletedEventArgs> FxpCopyAsyncCompleted;

		/// <summary>
		/// Asynchronous File Exchange Protocol (FXP) allows server-to-server transfer which can greatly speed up file transfers.
		/// </summary>
		/// <param name="fileName">The name of the file to transfer.</param>
		/// <param name="destination">The destination FTP server which must be supplied as an open and connected FtpClient object.</param>
		/// <remarks>
		/// Both servers must support and have FXP enabled before you can transfer files between two remote servers using FXP.  One FTP server must support PASV mode and the other server must allow PORT commands from a foreign address.  Finally, firewall settings may interfer with the ability of one server to access the other server.
		/// Starksoft FtpClient will coordinate the FTP negoitaion and necessary PORT and PASV transfer commands.
		/// </remarks>
		/// <seealso cref="FxpCopyAsyncCompleted"/>
		/// <seealso cref="FxpTransferTimeout"/>
		/// <seealso cref="FxpCopy"/>
		/// <seealso cref="FtpBase.CancelAsync"/>
		public void FxpCopyAsync(string fileName, FtpClient destination) {
			if (base.AsyncWorker != null && base.AsyncWorker.IsBusy)
				throw new InvalidOperationException("The FtpClient object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

			base.CreateAsyncWorker();
			base.AsyncWorker.WorkerSupportsCancellation = true;
			base.AsyncWorker.DoWork += new DoWorkEventHandler(FxpCopyAsync_DoWork);
			base.AsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FxpCopyAsync_RunWorkerCompleted);
			Object[] args = new Object[2];
			args[0] = fileName;
			args[1] = destination;
			base.AsyncWorker.RunWorkerAsync(args);
		}

		private void FxpCopyAsync_DoWork(object sender, DoWorkEventArgs e) {
			try {
				Object[] args = (Object[])e.Argument;
				FxpCopy((string)args[0], (FtpClient)args[1]);
			} catch (Exception ex) {
				_asyncException = ex;
			}
		}

		private void FxpCopyAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if (FxpCopyAsyncCompleted != null)
				FxpCopyAsyncCompleted(this, new FxpCopyAsyncCompletedEventArgs(_asyncException, base.IsAsyncCanceled));
			_asyncException = null;
		}

		#endregion

		#region Destructors

		/// <summary>
		/// Disposes all FtpClient objects and connections.
		/// </summary>
		new protected virtual void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose Method.
		/// </summary>
		/// <param name="disposing"></param>
		override protected void Dispose(bool disposing) {
			if (disposing) {
				// close any managed objects
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Dispose deconstructor.
		/// </summary>
		~FtpClient() {
			Dispose(false);
		}

		#endregion
	}
}




