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
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.IO;
using System.Text;
using System.Threading;
using System.Globalization;
using System.ComponentModel;
using System.IO.Compression;
using System.Diagnostics;
//using Starksoft.Net.Proxy;


namespace Starksoft.Net.Ftp
{
    #region Public Enums

    /// <summary>
    /// The type of data transfer mode (e.g. Active or Passive).
    /// </summary>
    /// <remarks>
    /// The default setting is Passive data transfer mode.  This mode is widely used as a
    /// firewall friendly setting for the FTP clients operating behind a firewall.
    /// </remarks>
    public enum TransferMode : int
    {
        /// <summary>
        /// Active transfer mode.  In this mode the FTP server initiates a connection to the client when transfering data.
        /// </summary>
        /// <remarks>This transfer mode may not work when the FTP client is behind a firewall and is accessing a remote FTP server.</remarks>
        Active,
        /// <summary>
        /// Passive transfer mode.  In this mode the FTP client initiates a connection to the server when transfering data.
        /// </summary>
        /// <remarks>
        /// This transfer mode is "firewall friendly" and generally allows an FTP client behind a firewall to access a remote FTP server.
        /// This mode is recommended for most data transfers.
        /// </remarks>
        Passive
    }

    /// <summary>
    /// The data transfer directory.
    /// </summary>
    internal enum TransferDirection : int
    {
        /// <summary>
        /// Transfer data from server to client.
        /// </summary>
        ToClient,
        /// <summary>
        /// Transfer data from client to server.
        /// </summary>
        ToServer
    }

    /// <summary>
    /// Enumeration representing the type of integrity algorithm used to verify the integrity of the file after transfer and storage.
    /// </summary>
    public enum HashingFunction : int
    {
        /// <summary>
        /// No algorithm slected.
        /// </summary>
        None,
        /// <summary>
        /// Cyclic redundancy check (CRC).  A CRC can be used in the same way as a checksum to detect accidental 
        /// alteration of data during transmission or storage.
        /// </summary>
        /// <remarks>
        /// It is often falsely assumed that when a message and its CRC are transmitted over an open channel, then when it arrives 
        /// if the CRC matches the message's calculated CRC then the message can not have been altered in transit.
        /// For this reason it is recommended to use SHA1 whenever possible.
        /// </remarks>
        /// <seealso cref="Sha1"/>
        Crc32,
        /// <summary>
        /// Message-Digest algorithm 5 (MD5).  Hashing function used to produce a 'unique' signature to detect 
        /// alternation of data during transmission or storage.
        /// </summary>
        /// <remarks>
        /// MD5 is a weak algorithm and has been show to produce collisions.  For this reason it is recommended to use SHA1 whenere possible.
        /// </remarks>
        /// <seealso cref="Sha1"/>
        Md5,
        /// <summary>
        /// Secure Hash Algorithm (SHA).  cryptographic hash functions designed by the National Security Agency (NSA) and published by the NIST as a U.S. Federal Information Processing Standard.
        /// </summary>
        /// <remarks>
        /// SHA1 is the recommended integrity check algorithm.  Even a small change in the message will, with overwhelming probability, result in a completely different hash due to the avalanche effect.
        /// </remarks>
        Sha1
    }

    #endregion

    #region  Public FTP Response Code Enum

    /// <summary>
    /// Enumeration representing all the various response codes from a FTP server.
    /// </summary>
    public enum FtpResponseCode : int
    {
        /// <summary>
        /// No response was received from the server.
        /// </summary>
        None = 0,
        /// <summary>
        /// The command was executed sucessfully (200).
        /// </summary>
        CommandOkay = 200,
        /// <summary>
        /// A syntax error occurred because the command was not recognized (500).
        /// </summary>
        SyntaxErrorCommandUnrecognized = 500,
        /// <summary>
        /// A syntax error occurred because the input parameters or arguments for the command are invalid (501).
        /// </summary>
        SyntaxErrorInParametersOrArguments = 501,
        /// <summary>
        /// The command is considered superfluous and not implemented by the FTP server (202).
        /// </summary>
        CommandNotImplementedSuperfluousAtThisSite = 202,
        /// <summary>
        /// The command is not implement by the FTP server (502).
        /// </summary>
        CommandNotImplemented = 502,
        /// <summary>
        /// A bad sequence of commands was issued (503).
        /// </summary>
        BadSequenceOfCommands = 503,
        /// <summary>
        /// The command does not support the supplied parameter (504).
        /// </summary>
        CommandNotImplementedForThatParameter = 504,
        /// <summary>
        /// Restart marker reply (110).  MARK yyyy = mmmm  Where yyyy is User-process data 
        /// stream marker, and mmmm server's equivalent marker (note the spaces between
        /// markers and "=").
        /// </summary>
        RestartMarkerReply = 110,
        /// <summary>
        /// System status or system help reply (211).
        /// </summary>
        SystemStatusOrHelpReply = 211,
        /// <summary>
        /// Directory status (212).
        /// </summary>
        DirectoryStatus = 212,
        /// <summary>
        /// File status (213).
        /// </summary>
        FileStatus = 213,
        /// <summary>
        /// Help message (214).  On how to use the server or the meaning of a particular
        /// non-standard command.  This reply is useful only to the human user.
        /// </summary>
        HelpMessage = 214,
        /// <summary>
        /// Name system type where Name is an official system name from the list in the
        /// Assigned Numbers document (215).
        /// </summary>
        NameSystemType = 215,
        /// <summary>
        /// Service ready in xxx minutes (120).
        /// </summary>
        ServiceReadyInxxxMinutes = 120,
        /// <summary>
        /// Service is now ready for new user (220).
        /// </summary>
        ServiceReadyForNewUser = 220,
        /// <summary>
        /// Service is closing control connection (221).
        /// </summary>
        ServiceClosingControlConnection = 221,
        /// <summary>
        /// Service not available, closing control connection (421). This may be a reply to any 
        /// command if the service knows it must shut down.
        /// </summary>
        ServiceNotAvailableClosingControlConnection = 421,
        /// <summary>
        /// Data connection already open; transfer starting (125).
        /// </summary>
        DataConnectionAlreadyOpenSoTransferStarting = 125,
        /// <summary>
        /// Data connection open so no transfer in progress (225).
        /// </summary>
        DataConnectionOpenSoNoTransferInProgress = 225,
        /// <summary>
        /// Can not open data connection (425).
        /// </summary>
        CannotOpenDataConnection = 425,
        /// <summary>
        /// Requested file action successful (for example, file transfer or file abort) (226).
        /// </summary>
        ClosingDataConnection = 226,
        /// <summary>
        /// Connection closed therefore the transfer was aborted (426).
        /// </summary>
        ConnectionClosedSoTransferAborted = 426,
        /// <summary>
        /// Entering Passive Mode (h1,h2,h3,h4,p1,p2) (227).
        /// </summary>
        EnteringPassiveMode = 227,
        /// <summary>
        /// User logged in, proceed (230).
        /// </summary>
        UserLoggedIn = 230,
        /// <summary>
        /// User is not logged in.  Command not accepted (530).
        /// </summary>
        NotLoggedIn = 530,
        /// <summary>
        /// The user name was accepted but the password must now be supplied (331).
        /// </summary>
        UserNameOkayButNeedPassword = 331,
        /// <summary>
        /// An account is needed for login (332).
        /// </summary>
        NeedAccountForLogin = 332,
        /// <summary>
        /// An account is needed for storing file on the server (532).
        /// </summary>
        NeedAccountForStoringFiles = 532,
        /// <summary>
        /// File status okay; about to open data connection (150).
        /// </summary>
        FileStatusOkaySoAboutToOpenDataConnection = 150,
        /// <summary>
        /// Requested file action okay, completed (250).
        /// </summary>
        RequestedFileActionOkayAndCompleted = 250,
        /// <summary>
        /// The pathname was created (257).
        /// </summary>
        PathNameCreated = 257,
        /// <summary>
        /// Requested file action pending further information (350).
        /// </summary>
        RequestedFileActionPendingFurtherInformation = 350,
        /// <summary>
        /// Requested file action not taken (450).  
        /// </summary>
        RequestedFileActionNotTaken = 450,
        /// <summary>
        /// Requested file action not taken (550).  File unavailable (e.g., file busy).
        /// </summary>
        RequestedActionNotTakenFileUnavailable = 550,
        /// <summary>
        /// Requested action aborted (451). Local error in processing.
        /// </summary>
        RequestedActionAbortedDueToLocalErrorInProcessing = 451,
        /// <summary>
        /// Requested action aborted (551). Page type unknown.
        /// </summary>
        RequestedActionAbortedPageTypeUnknown = 551,
        /// <summary>
        /// Requested action not taken (452).  Insufficient storage space in system.
        /// </summary>
        RequestedActionNotTakenInsufficientStorage = 452,
        /// <summary>
        /// Requested file action aborted (552).  Exceeded storage allocation (for current directory or dataset).
        /// </summary>
        RequestedFileActionAbortedExceededStorageAllocation = 552,
        /// <summary>
        /// Requested action not taken (553).  File name not allowed.
        /// </summary>
        RequestedActionNotTakenFileNameNotAllowed = 553,
        /// <summary>
        /// Secure authentication Okay (234).
        /// </summary>
        AuthenticationCommandOkay = 234,
        /// <summary>
        /// SSL service is Unavailable (431).
        /// </summary>
        ServiceIsUnavailable = 431
    }
    #endregion


    /// <summary>
    /// Defines the possible versions of FtpSecurityProtocol.
    /// </summary>
    public enum FtpSecurityProtocol : int
    {
        /// <summary>
        /// No security protocol specified.
        /// </summary>
        None,
        /// <summary>
        /// Specifies Transport Layer Security (TLS) version 1.0 is required to secure communciations.  The TLS protocol is defined in IETF RFC 2246 and supercedes the SSL 3.0 protocol.
        /// </summary>
        /// <remarks>
        /// The AUTH TLS command is sent to the FTP server to secure the connection.  TLS protocol is the latest version of the SSL protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// </remarks>
        Tls1Explicit,
        /// <summary>
        /// Specifies Transport Layer Security (TLS) version 1.0. or Secure Socket Layer (SSL) version 3.0 is acceptable to secure communications in explicit mode.
        /// </summary>
        /// <remarks>
        /// The AUTH SSL command is sent to the FTP server to secure the connection but the security protocol is negotiated between the server and client.  
        /// TLS protocol is the latest version of the SSL 3.0 protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// </remarks>
        Tls1OrSsl3Explicit,
        /// <summary>
        /// Specifies Secure Socket Layer (SSL) version 3.0 is required to secure communications in explicit mode.  SSL 3.0 has been superseded by the TLS protocol
        /// and is provided for backward compatibility only
        /// </summary>
        /// <remarks>
        /// The AUTH SSL command is sent to the FTP server to secure the connection.  TLS protocol is the latest version of the SSL 3.0 protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// Some FTP server do not implement TLS or understand the command AUTH TLS.  In those situations you should specify the security
        /// protocol Ssl3, otherwise specify Tls1.
        /// </remarks>
        Ssl3Explicit,
        /// <summary>
        /// Specifies Secure Socket Layer (SSL) version 2.0 is required to secure communications in explicit mode.  SSL 2.0 has been superseded by the TLS protocol
        /// and is provided for backward compatibility only.  SSL 2.0 has several weaknesses and should only be used with legacy FTP server that require it.
        /// </summary>
        /// <remarks>
        /// The AUTH SSL command is sent to the FTP server to secure the connection.  TLS protocol is the latest version of the SSL 3.0 protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// Some FTP server do not implement TLS or understand the command AUTH TLS.  In those situations you should specify the security
        /// protocol Ssl3, otherwise specify Tls1.
        /// </remarks>
        Ssl2Explicit,
        /// <summary>
        /// Specifies Transport Layer Security (TLS) version 1.0 is required to secure communciations in explicit mode.  The TLS protocol is defined in IETF RFC 2246 and supercedes the SSL 3.0 protocol.
        /// </summary>
        /// <remarks>
        /// The AUTH TLS command is sent to the FTP server to secure the connection.  TLS protocol is the latest version of the SSL protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// </remarks>
        Tls1Implicit,
        /// <summary>
        /// Specifies Transport Layer Security (TLS) version 1.0. or Secure Socket Layer (SSL) version 3.0 is acceptable to secure communications in implicit mode.
        /// </summary>
        /// <remarks>
        /// The AUTH SSL command is sent to the FTP server to secure the connection but the security protocol is negotiated between the server and client.  
        /// TLS protocol is the latest version of the SSL 3.0 protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// </remarks>
        Tls1OrSsl3Implicit,
        /// <summary>
        /// Specifies Secure Socket Layer (SSL) version 3.0 is required to secure communications in implicit mode.  SSL 3.0 has been superseded by the TLS protocol
        /// and is provided for backward compatibility only
        /// </summary>
        /// <remarks>
        /// The AUTH SSL command is sent to the FTP server to secure the connection.  TLS protocol is the latest version of the SSL 3.0 protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// Some FTP server do not implement TLS or understand the command AUTH TLS.  In those situations you should specify the security
        /// protocol Ssl3, otherwise specify Tls1.
        /// </remarks>
        Ssl3Implicit,
        /// <summary>
        /// Specifies Secure Socket Layer (SSL) version 2.0 is required to secure communications in implicit mode.  SSL 2.0 has been superseded by the TLS protocol
        /// and is provided for backward compatibility only.  SSL 2.0 has several weaknesses and should only be used with legacy FTP server that require it.
        /// </summary>
        /// <remarks>
        /// The AUTH SSL command is sent to the FTP server to secure the connection.  TLS protocol is the latest version of the SSL 3.0 protcol and is the security protocol that should be used whenever possible.
        /// There are slight differences between SSL version 3.0 and TLS version 1.0, but the protocol remains substantially the same.
        /// Some FTP server do not implement TLS or understand the command AUTH TLS.  In those situations you should specify the security
        /// protocol Ssl3, otherwise specify Tls1.
        /// </remarks>
        Ssl2Implicit
    }

    /// <summary>
    /// Base abstract class for FtpClient.  Implements FTP network protocols.
    /// </summary>
    public abstract class FtpBase : IDisposable
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the FtpNetworkAdapter class.
        /// </summary>
        /// <param name="port">Port number the adapter is to communicate on.</param>
        /// <param name="securityProtocol">Value indicating what secure security communications protocol should be used (if any).</param>
        internal FtpBase(int port, FtpSecurityProtocol securityProtocol)
        {
            _port = port;
            _securityProtocol = securityProtocol;
        }

        /// <summary>
        /// Initializes a new instance of the FtpNetworkAdapter class.
        /// </summary>
        /// <param name="host">Host the adapter is to communicate on.</param>
        /// <param name="port">Port number the adapter is to communicate on.</param>
        /// <param name="securityProtocol">Value indicating what secure security communications protocol should be used (if any).</param>
        internal FtpBase(string host, int port, FtpSecurityProtocol securityProtocol)
		{
			_host = host;
			_port = port;
            _securityProtocol = securityProtocol;
        }

        #endregion

		  public johnshope.Sync.SyncJob Job { get; set; }
        #region Private Variables and Constants

        private TcpClient _commandConn;
        private Stream _commandStream;
        private TcpClient _dataConn;

        private int _port;
        private string _host;
        private TransferMode _dataTransferMode = TransferMode.Passive;

        private FtpResponseQueue _responseQueue = new FtpResponseQueue();
        private FtpResponse _response = new FtpResponse();
        private FtpResponseCollection _responseList = new FtpResponseCollection();
        
        private Thread _responseMonitor;
        //static object _reponseMonitorLock = new object();
		object _reponseMonitorLock = new object();

        //private IProxyClient _proxy;
        private int _maxUploadSpeed;
        private int _maxDownloadSpeed;

        private int _tcpBufferSize = TCP_BUFFER_SIZE;
        private int _tcpTimeout = TCP_TIMEOUT;

        private int _transferTimeout = TRANSFER_TIMEOUT;
        private int _commandTimeout = COMMAND_TIMEOUT;

        private TcpListener _activeListener;
        private static int _activePort;
        private int _activePortRangeMin = 50000;
        private int _activePortRangeMax = 50080;
        
        // secure communications specific 
        private FtpSecurityProtocol _securityProtocol = FtpSecurityProtocol.None;
        private X509Certificate2 _serverCertificate;
        private X509CertificateCollection _clientCertificates = new X509CertificateCollection();

        // data compresion specific
        private bool _isCompressionEnabled;

        // data integrity specific
        private HashingFunction _hashAlgorithm;

        // character encoding
        private Encoding _encoding = Encoding.UTF8;

        // thread signal for active mode data transfer
        private ManualResetEvent _activeSignal = new ManualResetEvent(false);

        // async background worker event-based object
        private BackgroundWorker _asyncWorker;
        private bool _asyncCanceled;

        private const int TCP_BUFFER_SIZE = 8192; 
        private const int TCP_TIMEOUT = 30000; // 30 seconds

        private const int WAIT_FOR_DATA_INTERVAL = 10; // 10 ms
        private const int WAIT_FOR_COMMAND_RESPONSE_INTERVAL = 10; // 10 ms
        private const int TRANSFER_TIMEOUT = 15000; // 15 seconds
        private const int COMMAND_TIMEOUT = 15000; // 15 seconds

        #endregion

        #region Public Events

        /// <summary>
        /// Server response event.
        /// </summary>
        public event EventHandler<FtpResponseEventArgs> ServerResponse;

        /// <summary>
        /// Server request event.
        /// </summary>
        public event EventHandler<FtpRequestEventArgs> ClientRequest;

        /// <summary>
        /// Data transfer progress event.
        /// </summary>
        public event EventHandler<TransferProgressEventArgs> TransferProgress;

        /// <summary>
        /// Data transfer complete event.
        /// </summary>
        public event EventHandler<TransferCompleteEventArgs> TransferComplete;

        /// <summary>
        /// Security certificate authentication event.
        /// </summary>
        public event EventHandler<ValidateServerCertificateEventArgs> ValidateServerCertificate;
        
        /// <summary>
        /// Connection closed event.
        /// </summary>
		  public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancels any asychronous operation that is currently active.
        /// </summary>
        /// <seealso cref="FtpClient.FxpCopyAsync"/>
        /// <seealso cref="FtpClient.GetDirListAsync()"/>
        /// <seealso cref="FtpClient.GetDirListAsync(string)"/>
        /// <seealso cref="FtpClient.GetDirListDeepAsync()"/>
        /// <seealso cref="FtpClient.GetDirListDeepAsync(string)"/>
        /// <seealso cref="FtpClient.GetFileAsync(string, string, FileAction)"/>
        /// <seealso cref="FtpClient.GetFileAsync(string, Stream, bool)"/>
        /// <seealso cref="FtpClient.OpenAsync"/>
        /// <seealso cref="FtpClient.PutFileAsync(string, string, FileAction)"/>
        /// <seealso cref="FtpClient.PutFileAsync(string, FileAction)"/>
        public void CancelAsync()
        {
            if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
            {
                _asyncCanceled = true;
                _asyncWorker.CancelAsync();
            }
        }

        /// <summary>
        /// Gets the checksum value from the FTP server for the file specified.  Use this value to compare a local checksum to determine file integrity.
        /// </summary>
        /// <param name="hash">Hashing function to use.</param>
        /// <param name="path">Path to the file ont the remote FTP server.</param>
        /// <returns>Hash value in a string format.</returns>
        /// <seealso cref="ComputeChecksum(HashingFunction, string)"/>
        public string GetChecksum(HashingFunction hash, string path)
        {
            return GetChecksum(hash, path, 0, 0);
        }

        /// <summary>
        /// Gets the checksum hash value from the FTP server for the file specified.  Use this value to compare a local checksum to determine file integrity.
        /// </summary>
        /// <param name="hash">Hashing function to use.</param>
        /// <param name="path">Path to the file on the remote FTP server.</param>
        /// <param name="startPosition">Byte position of where the server should begin computing the hash.</param>
        /// <param name="endPosition">Byte position of where the server should end computing the hash.</param>
        /// <returns>Checksum hash value in a string format.</returns>
        /// <seealso cref="ComputeChecksum(HashingFunction, string)"/>
        public string GetChecksum(HashingFunction hash, string path, long startPosition, long endPosition)
        {
            if (hash == HashingFunction.None)
                throw new ArgumentOutOfRangeException("hash", "must contain a value other than 'Unknown'");

            if (startPosition < 0)
                throw new ArgumentOutOfRangeException("startPosition", "must contain a value greater than or equal to 0");

            if (endPosition < 0)
                throw new ArgumentOutOfRangeException("startPosition", "must contain a value greater than or equal to 0");

            if (startPosition > endPosition)
                throw new ArgumentOutOfRangeException("startPosition", "must contain a value less than or equal to endPosition");

            FtpCmd command = FtpCmd.Unknown;

            switch (hash)
            {
                case HashingFunction.Crc32:
                    command = FtpCmd.Xcrc;
                    break;

                case HashingFunction.Md5:
                    command = FtpCmd.Xmd5;
                    break;
                case HashingFunction.Sha1:
                    command = FtpCmd.Xsha1;
                    break;
            }

            // send request to server to get the hash value for the file
            // if the restartposition is > 0 then computer the hash on the segment that we resent
            if (startPosition > 0)
                SendRequest(new FtpRequest(_encoding, command, path, startPosition.ToString(), endPosition.ToString()));
            else
                SendRequest(new FtpRequest(_encoding, command, path));

            return _response.Text;
        }

        /// <summary>
        /// Computes a checksum for a local file.
        /// </summary>
        /// <param name="hash">Hashing function to use.</param>
        /// <param name="localPath">Path to file to perform checksum operation on.</param>
        /// <returns>Hash value in a string format.</returns>
        /// <seealso cref="GetChecksum(HashingFunction, string)"/>
        public string ComputeChecksum(HashingFunction hash, string localPath)
        {
            if (!File.Exists(localPath))
                throw new ArgumentException("file does not exist.", "localPath");
            
            using (FileStream fileStream = File.OpenRead(localPath))
            {
                return ComputeChecksum(hash, fileStream);
            }
        }

        /// <summary>
        /// Computes a checksum for a Stream object.
        /// </summary>
        /// <param name="hash">Hashing function to use.</param>
        /// <param name="inputStream">Any System.IO.Stream object.</param>
        /// <returns>Hash value in a string format.</returns>
        /// <remarks>
        /// The Stream object must allow reads and must allow seeking.
        /// </remarks>
        /// <seealso cref="GetChecksum(HashingFunction, string)"/>
        public string ComputeChecksum(HashingFunction hash, Stream inputStream)
        {
            return ComputeChecksum(hash, inputStream, 0);
        }

        /// <summary>
        /// Computes a checksum value for a Stream object.
        /// </summary>
        /// <param name="hash">Hashing function to use.</param>
        /// <param name="inputStream">Any System.IO.Stream object.</param>
        /// <param name="startPosition">Byte position of where the hash computation should begin.</param>
        /// <returns>Hash value in a string format.</returns>
        /// <remarks>
        /// The Stream object must allow reads and must allow seeking.
        /// </remarks>
        /// <seealso cref="GetChecksum(HashingFunction, string)"/>
        public static string ComputeChecksum(HashingFunction hash, Stream inputStream, long startPosition)
        {
            if (hash == HashingFunction.None)
                throw new ArgumentOutOfRangeException("hash", "must contain a value other than 'Unknown'");

            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (!inputStream.CanRead)
                throw new ArgumentException("must be readable.  The CanRead property must return a value of 'true'.", "inputStream");

            if (!inputStream.CanSeek)
                throw new ArgumentException("must be seekable.  The CanSeek property must return a value of 'true'.", "inputStream");

            if (startPosition < 0)
                throw new ArgumentOutOfRangeException("startPosition", "must contain a value greater than or equal to 0");

            HashAlgorithm hashAlgo = null;

            switch (hash)
            {
                case HashingFunction.Crc32:
                    hashAlgo = new Starksoft.Hashing.Crc32();
                    break;
                case HashingFunction.Md5:
                    hashAlgo = new MD5CryptoServiceProvider();
                    break;
                case HashingFunction.Sha1:
                    hashAlgo = new SHA1CryptoServiceProvider();
                    break;
            }

            if (startPosition > 0)
                inputStream.Position = startPosition;
            else
                inputStream.Position = 0;

            byte[] hashArray = hashAlgo.ComputeHash(inputStream);

            // convert byte array to a string
            StringBuilder buffer = new StringBuilder(hashArray.Length);
            foreach (byte hashByte in hashArray)
            {
                buffer.Append(hashByte.ToString("x2"));
            }

            return buffer.ToString();
        }

        #endregion

        #region Internal Properties
        
        internal BackgroundWorker AsyncWorker
        {
            get { return _asyncWorker; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation is canceled.
        /// </summary>
        /// <remarks>Returns true if an asynchronous operation is canceled; otherwise, false.
        /// </remarks>
        public bool IsAsyncCanceled
        {
            get { return _asyncCanceled; }
        }
        
        /// <summary>
        /// Gets a value indicating whether an asynchronous operation is running.
        /// </summary>
        /// <remarks>Returns true if an asynchronous operation is running; otherwise, false.
        /// </remarks>
        public bool IsBusy
        {
            get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
        }

        /// <summary>
        /// Gets or sets the current port number used by the FtpClient to make a connection to the FTP server.
        /// </summary>
        /// <remarks>
        /// The default value is '80'.  This setting can only be changed when the 
        /// connection to the FTP server is closed.  And FtpException is thrown if this 
        /// setting is changed when the FTP server connection is open.
        /// 
        /// Returns an integer representing the port number used to connect to a remote server.
        /// </remarks>
        public int Port
        {
            get { return _port; }
            set 
            {
                if (this.IsConnected)
                    throw new FtpException("Port property value can not be changed when connection is open.");

                _port = value; 
            }
        }

        /// <summary>
        /// Gets or sets a text value containing the current host used by the FtpClient to make a connection to the FTP server.
        /// </summary>
        /// <remarks>
        /// This value may be in the form of either a host name or IP address.
        /// This setting can only be changed when the 
        /// connection to the FTP server is closed.  And FtpException is thrown if this 
        /// setting is changed when the FTP server connection is open.
        /// 
        /// Returns a string with either the host name or host ip address.
        /// </remarks>
        public string Host
        {
            get { return _host; }
            set 
            {
                if (this.IsConnected)
                    throw new FtpException("Host property value can not be changed when connection is open.");

                _host = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what security protocol such as Secure Sock Layer (SSL) should be used.
        /// </summary>
        /// <remarks>
        /// The default value is 'None'.  This setting can only be changed when the 
        /// connection to the FTP server is closed.  An FtpException is thrown if this 
        /// setting is changed when the FTP server connection is open.
        /// 
        /// Returns an enumerator specifying the choosen security protocol of either TLS v1.0, SSL v3.0 or SSL v2.0.
        /// </remarks>
        /// <seealso cref="SecurityCertificates"/>
        /// <seealso cref="ValidateServerCertificate" />
        public FtpSecurityProtocol SecurityProtocol
        {
            get { return _securityProtocol; }
            set 
            {
                if (this.IsConnected)
                    throw new FtpException("SecurityProtocol property value can not be changed when connection is open.");

                _securityProtocol = value; 
            }
        }

        /// <summary>
        /// Get Client certificate collection used when connection with a secured SSL/TSL protocol.  Add your client certificates 
        /// if required to connect to the remote FTP server.
        /// </summary>
        /// <remarks>Returns a X509CertificateCollection list contains X.509 security certificates.</remarks>
        /// <seealso cref="SecurityProtocol"/>
        /// <seealso cref="ValidateServerCertificate" />
        public X509CertificateCollection SecurityCertificates
        {
            get { return _clientCertificates; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the client will use compression when uploading and downloading
        /// data.
        /// </summary>
        /// <remarks>
        /// This value turns on or off the compression algorithm DEFLATE to facility FTP data compression which is compatible with
        /// FTP servers that implement compression via the zLib compression software library.  The default value is 'False'.  
        /// This setting can only be changed when the system is not busy conducting other operations.  
        /// 
        /// Returns True if compression is enabled; otherwise False;
        /// </remarks>
        public bool IsCompressionEnabled
        {
            get { return _isCompressionEnabled; }
            set 
            { 
                if (this.IsBusy)
                    throw new FtpException("IsCompressionEnabled property value can not be changed when the system is busy.");

                try
                {
                    // enable compression
                    if (this.IsConnected && value && value != _isCompressionEnabled)
                        CompressionOn();

                    // disable compression
                    if (this.IsConnected && !value && value != _isCompressionEnabled)
                        CompressionOff();
                }
                catch (FtpException ex)
                {
                    throw new FtpDataCompressionException("An error occurred while trying to enable or disable FTP data compression.", ex);
                }
                
                _isCompressionEnabled = value; 
            }
        }

        /// <summary>
        /// Gets or sets an Integer value representing the maximum upload speed allowed 
        /// for data transfers in kilobytes per second.
        /// </summary>
        /// <remarks>
        /// Set this value when you would like to throttle back any upload data transfers.
        /// A value of zero means there is no restriction on how fast data uploads are 
        /// conducted.  The default value is zero.  This setting is used to throttle data traffic so the FtpClient does
        /// not consume all available network bandwidth.
        /// </remarks>
        /// <seealso cref="MaxDownloadSpeed"/>
        public int MaxUploadSpeed
        {
            get { return _maxUploadSpeed; }
            set 
            {
                if (value * 1024 > Int32.MaxValue || value < 0)
                    throw new ArgumentOutOfRangeException("value", "The MaxUploadSpeed property must have a range of 0 to 2,097,152.");
                
                _maxUploadSpeed = value; 
            }
        }

        /// <summary>
        /// Gets or sets an Integer value representing the maximum download speed allowed 
        /// for data transfers in kilobytes per second.
        /// </summary>
        /// <remarks>
        /// Set this value when you would like to throttle back any download data transfers.
        /// A value of zero means there is no restriction on how fast data uploads are 
        /// conducted.  The default value is zero.  This setting is used to throttle data traffic so the FtpClient does
        /// not consume all available network bandwidth.
        /// </remarks>
        /// <seealso cref="MaxUploadSpeed"/>
        public int MaxDownloadSpeed
        {
            get { return _maxDownloadSpeed; }
            set 
            {
                if (value * 1024 > Int32.MaxValue || value < 0)
                    throw new ArgumentOutOfRangeException("value", "must have a range of 0 to 2,097,152.");

                _maxDownloadSpeed = value; 
            }
        }

        /// <summary>
        /// Gets only the last response from the FTP server.
        /// </summary>
        /// <remarks>Returns a FtpResponse object containing the last FTP server response; other the value null (or Nothing in VB) is returned.</remarks>
        public FtpResponse LastResponse
        {
            get { return _response; }
        }

        /// <summary>
        /// Gets the list of all responses since the last command was issues to the server.
        /// </summary>
        /// <remarks>Returns a FtpResponseCollection list containing all the responses.</remarks>
        public FtpResponseCollection LastResponseList
        {
            get { return _responseList; }
        }

        /// <summary>
        /// Gets or sets the TCP buffer size used when communicating with the FTP server in bytes.
        /// </summary>
        /// <remarks>Returns an integer value representing the buffer size.  The default value is 8192.</remarks>
        public int TcpBufferSize
        {
            get { return _tcpBufferSize; }
            set 
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "must be greater than 0.");

                _tcpBufferSize = value; 
            }
        }

        /// <summary>
        /// Gets or sets the TCP timeout used when communciating with the FTP server in milliseconds.
        /// </summary>
        /// <remarks>
        /// Default value is 30000 (30 seconds).
        /// </remarks>
        /// <seealso cref="TransferTimeout"/>
        /// <seealso cref="CommandTimeout"/>
        public int TcpTimeout
        {
            get { return _tcpTimeout; }
            set 
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "must be greater than or equal to 0.");

                _tcpTimeout = value; 
            }
        }

        /// <summary>
        /// Gets or sets the data transfer timeout used when communicating with the FTP server in milliseconds.
        /// </summary>
        /// <remarks>
        /// Default value is 15000 (15 seconds).
        /// </remarks>
        /// <seealso cref="TcpTimeout"/>
        /// <seealso cref="CommandTimeout"/>
        public int TransferTimeout
        {
            get { return _transferTimeout; }
            set 
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "must be greater than 0.");
                
                _transferTimeout = value; 
            }
        }

        /// <summary>
        /// Gets or sets the FTP command timeout used when communciating with the FTP server in milliseconds.
        /// </summary>
        /// <remarks>
        /// Default value is 15000 (15 seconds).
        /// </remarks>
        /// <seealso cref="TcpTimeout"/>
        /// <seealso cref="TransferTimeout"/>
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set 
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "must be greater than 0.");
            
                _commandTimeout = value; 
            }
        }             

        /// <summary>
        /// The beginning port number range used by the FtpClient when opening a local 'Active' port.  The default value is 4051.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value must be less than or equal to the ActivePortRangeMax value.</exception>
        /// <remarks>
        /// When the FtpClient is in 'Active' mode a local port is opened for communications from the FTP server.
        /// The FtpClient will attempt to open an unused TCP listener port between the ActivePortRangeMin and ActivePortRangeMax values.
        /// Default value is 50000.
        /// </remarks>
        /// <seealso cref="ActivePortRangeMax"/>
        /// <seealso cref="DataTransferMode"/>
        public int ActivePortRangeMin
        {
            get { return _activePortRangeMin; }
            set
            {
                if (value > _activePortRangeMin)
                    throw new ArgumentOutOfRangeException("value","must be less than the ActivePortRangeMax value.");

                if (value < 1 || value > 65534)
                    throw new ArgumentOutOfRangeException("value", "must be between 1 and 65534.");

                if (this.IsBusy)
                    throw new FtpException("ActivePortRangeMin property value can not be changed when the component is busy.");

                _activePortRangeMin = value;
            }
        }

        /// <summary>
        /// The ending port number range used by the FtpClient when opening a local 'Active' port.  The default value is 4080.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to the ActivePortRangeMin value.</exception>
        /// <remarks>
        /// When the FtpClient is in 'Active' mode a local port is opened for communications from the FTP server.
        /// The FtpClient will attempt to open an unused TCP listener port between the ActivePortRangeMin and ActivePortRangeMax values.
        /// Default value is 50080.
        /// </remarks>
        /// <seealso cref="ActivePortRangeMin"/>
        /// <seealso cref="DataTransferMode"/>
        public int ActivePortRangeMax
        {
            get { return _activePortRangeMax; }
            set
            {
                if (value < _activePortRangeMin)
                    throw new ArgumentOutOfRangeException("value", "must be greater than the ActivePortRangeMin value.");

                if (value < 1 || value > 65534)
                    throw new ArgumentOutOfRangeException("value", "must be between 1 and 65534.");

                if (this.IsBusy)
                    throw new FtpException("ActivePortRangeMax property value can not be changed when the component is busy.");

                _activePortRangeMax = value;
            }
        }


        /// <summary>
        /// Gets or sets the data transfer mode to either Active or Passive.
        /// </summary>
        /// <seealso cref="ActivePortRangeMin"/>
        /// <seealso cref="ActivePortRangeMax"/>
        public TransferMode DataTransferMode
        {
            get { return _dataTransferMode; }
            set 
            {
                if (this.IsBusy)
                    throw new FtpException("DataTransferMode property value can not be changed when the component is busy.");

                _dataTransferMode = value; 
            }
        }

        /// <summary>
        /// Gets or sets the the proxy object to use when establishing a connection to the remote FTP server.
        /// </summary>
        /// <remarks>Create a proxy object when traversing a firewall.</remarks>
        /// <code>
        /// FtpClient ftp = new FtpClient();
        ///
        /// // create an instance of the client proxy factory for the an ftp client
        /// ftp.Proxy = (new ProxyClientFactory()).CreateProxyClient(ProxyType.Http, "localhost", 6588);
        ///        
        /// </code>
        /// <seealso cref="Starksoft.Net.Proxy.ProxyClientFactory"/>
        /* de
		  public IProxyClient Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }
		  */

        /// <summary>
        /// Gets the connection status to the FTP server.
        /// </summary>
        /// <remarks>Returns True if the connection is open; otherwise False.</remarks>
        /// <seealso cref="ConnectionClosed"/>
        public bool IsConnected
        {
            get 
            {
                if (_commandConn == null || _commandConn.Client == null)
                    return false;

                Socket client = _commandConn.Client;

                if (!client.Connected)
                    return false;
                
                // this is how you can determine whether a socket is still connected.
                bool blockingState = client.Blocking;
                bool connected = true;
                try
                {
                    byte[] tmp = new byte[1];

                    client.Blocking = false;
                    client.Send(tmp, 0, 0);
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (!e.NativeErrorCode.Equals(10035))
                    {
                        connected = false;
                    }
                }
                catch (ObjectDisposedException)
                {
                    connected = false;
                }
                finally
                {
                    try
                    {
                        client.Blocking = blockingState;
                    }
                    catch
                    {
                        connected = false;
                    }
                }

                return connected;

            }
        }

        /// <summary>
        /// Sets the automatic file integrity setting (checksum) option on all data transfers (upload and download).
        /// </summary>
        /// <remarks>
        /// The FtpClient library will throw an FtpFileIntegrityException if the file integrity value do not match.
        /// 
        /// Not all FTP servers support file integrity values such as SHA1, CRC32, or MD5.  If you server does support
        /// one of these file integrity options, you can set this property and the FtpClient will automatically check
        /// each file that is transferred to make sure the hash values match.  If the values do not match, an exception
        /// is thrown.
        /// </remarks>
        /// <seealso cref="FtpFileIntegrityException"/>
        /// <seealso cref="GetChecksum(HashingFunction, string)"/>
        /// <seealso cref="ComputeChecksum(HashingFunction, string)"/>
        public HashingFunction AutoChecksumValidation
        {
            get { return _hashAlgorithm; }
            set { _hashAlgorithm = value; }
        }

        /// <summary>
        /// Gets or sets the character encoding used when sending commands to the FTP server or receiving directory listing information.
        /// This encoding value does not affect the encoding of files being transferred.  The default value is UTF-8.  Some older FTP servers
        /// require different character encoding such as UTF-7.
        /// </summary>
        /// <remarks>
        /// To set this value use the .NET System.Text.Encoding class. The following example sets the character encoding to UTF-7.  In addition,
        /// other encodings can be specified by string name using the static method System.Text.Encoding.GetEncoding();
        /// <code>
        ///     FtpClient.CharacterEncoding = System.Text.Encoding.UTF7;
        /// </code>
        /// </remarks>
        public Encoding CharacterEncoding
        {
            get { return _encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("CharacterEncoding");
                _encoding = value;
            }
        }

		public string Home { get; set; }

        #endregion

        #region Internal Protected Methods

        /// <summary>
        /// Send a FTP command request to the server.
        /// </summary>
        /// <param name="request"></param>
        internal void SendRequest(FtpRequest request)
        {
            if (_commandConn == null || _commandConn.Connected == false)
                throw new FtpConnectionClosedException("Connection is closed.");

            // clear out any responses that might have been pending from a previous
            // failed operation
            DontWaitForHappyCodes();

            if (ClientRequest != null)
                ClientRequest(this, new FtpRequestEventArgs(request));

            byte[] buffer = request.GetBytes();

            try
            {
                _commandStream.Write(buffer, 0, buffer.Length);
            }
            catch (IOException ex)
            {
                throw new FtpConnectionBrokenException("Connection is broken.  Failed to send command.", ex);
            }

            // most commands will have happy codes but the quote() command does not 
            if (request.HasHappyCodes)
            {
                WaitForHappyCodes(request.GetHappyCodes());
            }
            else
            {
                // when there are no happy codes given the we have to give the server some time to response
                // since we really don't know what response is the correct one
                if (request.Command != FtpCmd.Quit)
                    Thread.Sleep(2000);
                DontWaitForHappyCodes();
            }
        }

        private void DontWaitForHappyCodes()
        {
            if (_responseQueue.Count == 0)
                return;

            _responseList.Clear();
            while (_responseQueue.Count > 0)
            {
                FtpResponse response = _responseQueue.Dequeue();
                _responseList.Add(response);
                RaiseServerResponseEvent(new FtpResponse(response));
            }
            _response = _responseList.GetLast();
        }




        /// <summary>
        /// creates a new async worker object for the async events to use.
        /// </summary>
        internal void CreateAsyncWorker()
        {
            if (_asyncWorker != null)
                _asyncWorker.Dispose();
            _asyncWorker = null;
            _asyncCanceled = false;
            _asyncWorker = new BackgroundWorker();
        }
                
        /// <summary>
        /// Closes all connections to the FTP server.
        /// </summary>
        internal void CloseAllConnections()
        {
            CloseDataConn();
            CloseCommandConn();
            AbortMonitorThread();
        }

        /// <summary>
        /// The monitor thread should close automatically once the command connection is terminated.  If it does not close properly, force it to close.
        /// </summary>
        private void AbortMonitorThread()
        {
            _responseMonitor.Abort();
        }

        //  open a connection to the server
        internal void OpenCommandConn()
        {
            //  create a new tcp client object 
            CreateCommandConnection();
            StartCommandMonitorThread();

            if (_securityProtocol == FtpSecurityProtocol.Ssl2Explicit || _securityProtocol == FtpSecurityProtocol.Ssl3Explicit || _securityProtocol == FtpSecurityProtocol.Tls1Explicit || _securityProtocol == FtpSecurityProtocol.Tls1OrSsl3Explicit)
                CreateSslExplicitCommandStream();

            if (_securityProtocol == FtpSecurityProtocol.Ssl2Implicit || _securityProtocol == FtpSecurityProtocol.Ssl3Implicit || _securityProtocol == FtpSecurityProtocol.Tls1Implicit || _securityProtocol == FtpSecurityProtocol.Tls1OrSsl3Implicit)
                CreateSslImplicitCommandStream();

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (IsAsyncCancellationPending())
                return;

            // this check screws up secure connections so we have to ignore it when secure connections are enabled
            if (_securityProtocol == FtpSecurityProtocol.None)
                WaitForHappyCodes(FtpResponseCode.ServiceReadyForNewUser);

        }

        internal void TransferData(TransferDirection direction, FtpRequest request, Stream data)
        {
            TransferData(direction, request, data, 0);
        }

        internal void TransferData(TransferDirection direction, FtpRequest request, Stream data, long restartPosition)
        {
            if (_commandConn == null || _commandConn.Connected == false)
                throw new FtpConnectionClosedException("Connection is closed.");

            if (request == null)
                throw new ArgumentNullException("request", "value is required");

            if (data == null)
                throw new ArgumentNullException("data", "value is required");

            switch (direction)
            {
                case TransferDirection.ToClient:
                    if (!data.CanWrite)
                        throw new FtpDataTransferException("Data transfer error.  Data conn does not allow write operation.");
                    break;
                case TransferDirection.ToServer:
                    if (!data.CanRead)
                        throw new FtpDataTransferException("Data transfer error.  Data conn does not allow read operation.");
                    break;
            }
            
            // if this is a restart then the data stream must support seeking
            if (restartPosition > 0 && !data.CanSeek)
                throw new FtpDataTransferException("Data transfer restart error.  Data conn does not allow seek operation.");

            try
            {
                // create a thread to begin the process of opening a data connection to the remote server
                OpenDataConn();

                //  check for a restart position 
                if (restartPosition > 0)
                {
                    // instruct the server to restart file transfer at the same position where the output stream left off
                    SendRequest(new FtpRequest(_encoding, FtpCmd.Rest, restartPosition.ToString(CultureInfo.InvariantCulture)));
                    
                    // set the data stream to the same position as the server
                    data.Position = restartPosition;
                }

                // send the data transfer command that requires a separate data connection to be established to transmit data
                SendRequest(request);

                // wait for the data connection thread to signal back that a connection has been established
                WaitForDataConn();

                // test to see if the data connection failed to be established sometime the active connection fails due to security settings on the ftp server
                if (_dataConn == null)
                    throw new FtpDataConnectionException("Unable to establish a data connection to the destination.  The destination may have refused the connection.");

                // create the data stream object - handles creation of SslStream and DeflateStream objects as well
                Stream conn = _dataConn.GetStream();

                // test to see if we need to enable ssl/tls explicit mode
                if (_securityProtocol != FtpSecurityProtocol.None)
                {
                    conn = CreateSslStream(conn);
                }
                
                // test to see if we need to enable compression by using the DeflateStream
                if (_isCompressionEnabled)
                {
                    conn = CreateZlibStream(direction, conn);
                }

                // based on the direction of the data transfer we need to handle the input and output streams
                switch (direction)
                {
                    case TransferDirection.ToClient:
                        TransferBytes(conn, data, _maxDownloadSpeed * 1024);
                        break;
                    case TransferDirection.ToServer:
                        TransferBytes(data, conn, _maxUploadSpeed * 1024);
                        break;
                }
            }
            finally
            {
                // attempt to close the data connection
                CloseDataConn();
            }

            // if no errors occurred and this is not a quoted command then we will wait for the server to send a closing connection message
            WaitForHappyCodes(FtpResponseCode.ClosingDataConnection);

            // integrity check
            if (_hashAlgorithm != HashingFunction.None && request.IsFileTransfer)
                DoIntegrityCheck(request, data, restartPosition);
        }

        private Stream CreateZlibStream(TransferDirection direction, Stream stream)
        {
            DeflateStream deflateStream = null;

            switch (direction)
            {
                case TransferDirection.ToClient:
                    deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true);
                    // zlib fix to ignore first two bytes of header data 
                    deflateStream.BaseStream.ReadByte();
                    deflateStream.BaseStream.ReadByte();
                    break;

                case TransferDirection.ToServer:
                    deflateStream = new DeflateStream(stream, CompressionMode.Compress, true);
                    // this is a fix for the DeflateStream class only when sending compressed data to the server.  
                    // Zlib has two bytes of data attached to the header that we have to write before processing the data stream.
                    deflateStream.BaseStream.WriteByte(120);
                    deflateStream.BaseStream.WriteByte(218);
                    break;
            }
            stream = deflateStream;
            return stream;
        }

        internal string TransferText(FtpRequest request)
        {
            Stream output = new MemoryStream();
            TransferData(TransferDirection.ToClient, request, output);
            output.Position = 0;
            StreamReader reader = new StreamReader(output, _encoding);
            return reader.ReadToEnd();
        }

        internal void CompressionOn()
        {
            try
            {
                SendRequest(new FtpRequest(_encoding, FtpCmd.Mode, "Z"));
            }
            catch (Exception ex)
            {
                throw new FtpDataCompressionException("Unable to enable compression (MODE Z) on the destination.", ex);
            }
        }

        internal void CompressionOff()
        {
            try
            {
                SendRequest(new FtpRequest(_encoding, FtpCmd.Mode, "S"));
            }
            catch (Exception ex)
            {
                throw new FtpDataCompressionException("Unable to disable compression (MODE S) on the destination.", ex);
            }
        }

        #endregion

        #region Private Methods

        private void StartCommandMonitorThread()
        {
            // start the monitor thread which pumps FtpResponse objects on the FtpResponseQueue
            _responseMonitor = new Thread(new ThreadStart(MonitorCommandConnection));
            _responseMonitor.Name = "FtpBase Response Monitor";
            _responseMonitor.Start();
        }

        private bool IsAsyncCancellationPending()
        {
            if (_asyncWorker != null && _asyncWorker.CancellationPending)
            {
                _asyncCanceled = true;
                return true;
            }
            return false;
        }

		public TimeSpan dw, dr;

        private void TransferBytes(Stream input, Stream output, int maxBytesPerSecond)
        {
            int bufferSize = _tcpBufferSize > maxBytesPerSecond && maxBytesPerSecond != 0 ? maxBytesPerSecond : _tcpBufferSize;
            byte[] buffer = new byte[bufferSize];
            long bytesTotal = 0;
            int bytesRead = 0;
            DateTime start = DateTime.Now;
            TimeSpan elapsed = new TimeSpan(0);
            int bytesPerSec = 0;

            while(true)
            {
				var t0 = DateTime.Now;
				bytesRead = input.Read(buffer, 0, bufferSize);
				var t1 = DateTime.Now;

                if (bytesRead == 0)
                    break;

                bytesTotal += bytesRead;

				var t2 = DateTime.Now;
				output.Write(buffer, 0, bytesRead);
				var t3 = DateTime.Now;

                // calculate some statistics
                elapsed = DateTime.Now.Subtract(start);
                bytesPerSec = (int)(elapsed.TotalSeconds < 1 ? bytesTotal : bytesTotal / elapsed.TotalSeconds);

				dr += (t1-t0);
				dw += (t3-t2);

                //  if the consumer subscribes to transfer progress event then fire it
                if (TransferProgress != null)
                    TransferProgress(this, new TransferProgressEventArgs(bytesRead, bytesPerSec, elapsed));

                // test to see if this is an asychronous operation and if so make sure 
                // the user has not requested the operation to be canceled
                if (IsAsyncCancellationPending())
                    throw new FtpAsynchronousOperationException("Asynchronous operation canceled by user.");

                // throttle the transfer if necessary
                ThrottleByteTransfer(maxBytesPerSecond, bytesTotal, elapsed, bytesPerSec);

            } ;

			// if (bytesTotal == 0) output.Write(buffer, 0, 0);

            //  if the consumer subscribes to transfer complete event then fire it
            if (TransferComplete != null)
                TransferComplete(this, new TransferCompleteEventArgs(bytesTotal, bytesPerSec, elapsed));
        }

        private void ThrottleByteTransfer(int maxBytesPerSecond, long bytesTotal, TimeSpan elapsed, int bytesPerSec)
        {
            // we only throttle if the maxBytesPerSecond is not zero (zero turns off the throttle)
            if (maxBytesPerSecond > 0)
            {
                // we only throttle if our through-put is higher than what we want
                if (bytesPerSec > maxBytesPerSecond)
                {
                    double elapsedMilliSec = elapsed.TotalSeconds == 0 ? elapsed.TotalMilliseconds : elapsed.TotalSeconds * 1000;

                    // need to calc a delay in milliseconds for the throttle wait based on how fast the 
                    // transfer is relative to the speed it needs to be
                    double millisecDelay = (bytesTotal / (maxBytesPerSecond / 1000) - elapsedMilliSec);
                    
                    // can only sleep to a max of an Int32 so we need to check this since bytesTotal is a long value
                    // this should never be an issue but never say never
                    if (millisecDelay > Int32.MaxValue)
                        millisecDelay = Int32.MaxValue;

                    // go to sleep
                    Thread.Sleep((int)millisecDelay);
                }
            }
        }

        private void CreateCommandConnection()
        {
            if (_host == null || _host.Length == 0)
                throw new FtpException("An FTP Host must be specified before opening connection to FTP destination.  Set the appropriate value using the Host property on the FtpClient object.");
            
            try {
                //  test to see if we should use the user supplied proxy object
                //  to create the connection
                /* if (_proxy != null)
                    _commandConn = _proxy.CreateConnection(_host, _port);
                else */
                    _commandConn = new TcpClient(_host, _port);
            } /*
            catch (ProxyException pex)
            {
                if (_commandConn != null)
                    _commandConn.Close();

                throw new FtpProxyException(String.Format(CultureInfo.InvariantCulture, "A proxy error occurred while creating connection to FTP destination {0} on port {1}.", _host, _port.ToString(CultureInfo.InvariantCulture)), pex);
            }*/
            catch (Exception ex)
            {
                if (_commandConn != null)
                    _commandConn.Close();

                throw new FtpConnectionOpenException(String.Format(CultureInfo.InvariantCulture, "An error occurred while opening a connection to FTP destination {0} on port {1}.", _host, _port.ToString(CultureInfo.InvariantCulture)), ex);
            }

            // set command connection buffer sizes and timeouts
            _commandConn.ReceiveBufferSize = _tcpBufferSize;
            _commandConn.ReceiveTimeout = _tcpTimeout;
            _commandConn.SendBufferSize = _tcpBufferSize;
            _commandConn.SendTimeout = _tcpTimeout;

            // set the command stream object
            _commandStream = _commandConn.GetStream();
        }
        



        private void CloseCommandConn()
        {
            if (_commandConn == null) 
                return;
            try
            {
                if (_commandConn.Connected)
                {
                    //  send the quit command to the server
                    SendRequest(new FtpRequest(_encoding, FtpCmd.Quit));
                }
                _commandConn.Close();
            }
            catch { }
            
            _commandConn = null;
        }


        private void WaitForHappyCodes(params FtpResponseCode[] happyResponseCodes)
        {
            WaitForHappyCodes(_commandTimeout, happyResponseCodes);
        }

        /// <summary>
        /// Waits until a happy code has been returned by the FTP server or times out.
        /// </summary>
        /// <param name="timeout">Maximum time to wait before timing out.</param>
        /// <param name="happyResponseCodes">Server response codes to wait for.</param>
        internal protected  void WaitForHappyCodes(int timeout, params FtpResponseCode[] happyResponseCodes)
        {
            _responseList.Clear();
            do
            {
                FtpResponse response = GetNextCommandResponse(timeout);
                _responseList.Add(response);
                RaiseServerResponseEvent(new FtpResponse(response));

                if (!response.IsInformational)
                {
                    if (IsHappyResponse(response, happyResponseCodes))
                        break;

                    if (IsUnhappyResponse(response))
                    {
                        _response = response;
                        throw new FtpResponseException("FTP command failed.", response);
                    }
                }
            } while (true);

            _response = _responseList.GetLast();
        }

        private void RaiseServerResponseEvent(FtpResponse response)
        {
            if (ServerResponse != null)
                ServerResponse(this, new FtpResponseEventArgs(response));
        }

        private void RaiseConnectionClosedEvent()
        {
            if (ConnectionClosed != null)
                ConnectionClosed(this, new ConnectionClosedEventArgs());
        }

        private bool IsUnhappyResponse(FtpResponse response)
        {
            if (
                 response.Code == FtpResponseCode.ServiceNotAvailableClosingControlConnection
                || response.Code == FtpResponseCode.CannotOpenDataConnection
                || response.Code == FtpResponseCode.ConnectionClosedSoTransferAborted
                || response.Code == FtpResponseCode.RequestedFileActionNotTaken
                || response.Code == FtpResponseCode.RequestedActionAbortedDueToLocalErrorInProcessing
                || response.Code == FtpResponseCode.RequestedActionNotTakenInsufficientStorage
                || response.Code == FtpResponseCode.SyntaxErrorCommandUnrecognized
                || response.Code == FtpResponseCode.SyntaxErrorInParametersOrArguments
                || response.Code == FtpResponseCode.CommandNotImplemented
                || response.Code == FtpResponseCode.BadSequenceOfCommands
                || response.Code == FtpResponseCode.CommandNotImplementedForThatParameter
                || response.Code == FtpResponseCode.NotLoggedIn
                || response.Code == FtpResponseCode.NeedAccountForStoringFiles
                || response.Code == FtpResponseCode.RequestedActionNotTakenFileUnavailable
                || response.Code == FtpResponseCode.RequestedActionAbortedPageTypeUnknown
                || response.Code == FtpResponseCode.RequestedFileActionAbortedExceededStorageAllocation
                || response.Code == FtpResponseCode.RequestedActionNotTakenFileNameNotAllowed)
                return true;
            else
                return false;
        }

        private bool IsHappyResponse(FtpResponse response, FtpResponseCode[] happyResponseCodes)
        {
            // always return true if there are no responses to validate
            if (happyResponseCodes.Length == 0)
                return true;

            for (int j = 0; j < happyResponseCodes.Length; j++)
            {
                if (happyResponseCodes[j] == response.Code)
                    return true;
            }
            return false;
        }

        private void MonitorCommandConnection()
        {
            byte[] buffer = new byte[_tcpBufferSize];
            StringBuilder response = new StringBuilder();
            while (IsConnected)
            {
                lock (_reponseMonitorLock)
                {
                    Thread.Sleep(WAIT_FOR_COMMAND_RESPONSE_INTERVAL);
                    try {
                        if (_commandConn != null && _commandConn.GetStream().DataAvailable) {
                            int bytes = _commandStream.Read(buffer, 0, _tcpBufferSize);
                            string partial = _encoding.GetString(buffer, 0, bytes);
                            response.Append(partial);
                            if (!partial.EndsWith("\r\n")) {
                                continue;
                            }

                            //  parse out the response code sent back from the server
                            //  in some cases more than one response can be sent with
                            //  each line separated with a crlf pair.
                            string[] responseArray = SplitResponse(response.ToString());
                            for (int i = 0; i < responseArray.Length; i++) {
                                _responseQueue.Enqueue(new FtpResponse(responseArray[i]));
                            }

                            response.Remove(0, response.Length);
                        }
                    } catch (ThreadAbortException ex) {
                        throw ex;
                    } catch { }
                }
            }

            RaiseConnectionClosedEvent();

        }

        private FtpResponse GetNextCommandResponse(int timeout)
        {
            int sleepTime = 0;
            while (_responseQueue.Count == 0)
            {
                if (!IsConnected)
                    throw new FtpConnectionClosedException("Connection is closed.");

                if (IsAsyncCancellationPending())
                    throw new FtpAsynchronousOperationException("Asynchronous operation canceled.");

                Thread.Sleep(WAIT_FOR_DATA_INTERVAL);
                sleepTime += WAIT_FOR_DATA_INTERVAL;
                if (sleepTime > timeout)
                    throw new FtpCommandResponseTimeoutException("A timeout occurred while waiting for the destination to send a response.  The last reponse from the destination is '" + _response.Text + "'");
            }

            // return next response object from the queue
            return _responseQueue.Dequeue();
        }

        private string[] SplitResponse(string response)
        {
            return response.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private int GetNextActiveModeListenerPort()
        {
            if (_activePort < _activePortRangeMin || _activePort > _activePortRangeMax)
                _activePort = _activePortRangeMin;
            else
                _activePort++;

            return _activePort;
        }

        private void CreateActiveConn()
        {
			if (_dataConn != null) return;

            string localHost = Dns.GetHostName();
            IPAddress[] localAddresses = Dns.GetHostAddresses(localHost);
            IPAddress localAddr = null;
            foreach (IPAddress addr in localAddresses)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddr = addr;
                }
            }

            if (localAddr == null)
            {
                throw new Exception("Local host does not have an IPv4 address");
            }

           
            // Set the event to nonsignaled state.
            _activeSignal.Reset();

            bool success = false;
            int listenerPort = 0;

            do
            {
                int failureCnt = 0;

                try
                {
                    listenerPort = GetNextActiveModeListenerPort();
                    _activeListener = new TcpListener(localAddr, listenerPort);
                    _activeListener.Start();
                    success = true;
                }
                catch (SocketException socketError)
                {
                    if (socketError.ErrorCode == 10048 && ++failureCnt < _activePortRangeMax - _activePortRangeMin)
                        _activeListener.Stop();
                    else
                        throw new FtpDataConnectionException(String.Format(CultureInfo.InvariantCulture, "An error occurred while trying to create an active connection on host {0} port {1}", localHost, listenerPort.ToString(CultureInfo.InvariantCulture)), socketError);
                }
            } while (!success);

            byte[] addrBytes = localAddr.GetAddressBytes();
            string dataPortInfo = String.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4},{5}", addrBytes[0].ToString(CultureInfo.InvariantCulture), addrBytes[1].ToString(CultureInfo.InvariantCulture), addrBytes[2].ToString(CultureInfo.InvariantCulture), addrBytes[3].ToString(CultureInfo.InvariantCulture), listenerPort / 256, listenerPort % 256);

            // Accept the connection.  BeginAcceptSocket() creates the accepted socket.
            _activeListener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpClientCallback), _activeListener);

				Job.Log.Debug("Opening Active Data Connection on Port " + listenerPort.ToString());

            //  send a command to the server instructing it to connect to
            //  the local ip address and port that the tcplistener is bound to
            try
            {
                SendRequest(new FtpRequest(_encoding, FtpCmd.Port, dataPortInfo));
            }
            catch (FtpException fex)
            {
                throw new FtpDataConnectionException(String.Format("An error occurred while issuing data port command '{0}' on an active FTP connection.", dataPortInfo), fex);
            }
        }

        // async callback that occurs once the server has connected to the client listener data connection
        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // make sure that the server didn't close the connection on us or just refuse to allow an active connection
            // for security considerations and other purposes some servers will refuse active connections
            try
            {
                _dataConn = listener.EndAcceptTcpClient(ar);
            }
            catch { }

            // signal the calling thread to continue now that the data connection is open
            _activeSignal.Set();       
        }

        private void OpenDataConn()
        {
            //  create the approiate ftp data connection depending on how the ftp client should send 
            //  or receive data from the ftp server
            if (_dataTransferMode == TransferMode.Active)
                CreateActiveConn();
            else
                CreatePassiveConn();
        }

        private void CloseDataConn()
        {
            //  close the tcpclient data connection object
            if (_dataConn != null)
            {
                try
                {

					//johnshope.Sync.Log.Debug("Close Connection.");

                    _dataConn.Close();
                }
                catch  { }
                _dataConn = null;
            }

            //  stop the tcplistner object if we used it for an active data transfer where we
            //  are listing and the server makes a connection to the client and pushed data 
            if (_dataTransferMode == TransferMode.Active && _activeListener != null)
            {
                try
                {
                    _activeListener.Stop();
                }
                catch { }
                _activeListener = null;
            }
        }

        private void WaitForDataConn()
        {
            //  if the transfer mode is active then we have to open a listener and wait for the server to connect before sending
            //  or receiving data but if the transfer mode is passive then we make the connection (non blocking) 
            //  and therefore there is no need to wait for the server and our signal that the server has connected 
            //  - we already have the connection object since the tcpclient object blocks until the server accepts the connection
            if (_dataTransferMode == TransferMode.Active)
            {
                // wait until a data connection is made before continuing based on a thread blocking signal object
                if (!_activeSignal.WaitOne(_transferTimeout, false))
                {
                    if (_response.Code == FtpResponseCode.CannotOpenDataConnection)
                        throw new FtpDataConnectionException(String.Format(CultureInfo.InvariantCulture, "The ftp destination was unable to open a data connection to the ftp client on port {0}.", _activePort));
                    else
                        throw new FtpDataConnectionTimeoutException("The data connection timed out waiting for data to transfer from the destination.");
                }
            }
            else
                return;
        }

        private void CreatePassiveConn()
        {
			if (_dataConn != null) return;

            //  send command to get passive port to be used from the server
            try
            {
                SendRequest(new FtpRequest(_encoding, FtpCmd.Pasv));
            }
            catch (FtpException fex)
            {
                throw new FtpDataConnectionException("An error occurred while issuing up a passive FTP connection command.", fex);
            }

            //  get the port on the end
            //  to calculate the port number we must multiple the 5th value by 256 and then add the 6th value
            //  example:  
            //       Client> PASV
            //       Server> 227 Entering Passive Mode (123,45,67,89,158,26)
            //  In the example of PASV mode the server has said it will be listening on IP address 123.45.67.89 
            //  on TCP port 40474 for the data channel. (Note: the destinationPort is the 158,26 pair and is: 158x256 + 26 = 40474).

            //  get the begin and end positions to extract data from the response string
            int startIdx = _response.Text.IndexOf("(") + 1;
            int endIdx = _response.Text.IndexOf(")");

            //  parse the transfer connection data from the ftp server response
            string[] data = _response.Text.Substring(startIdx, endIdx - startIdx).Split(',');

            // build the data host name from the server response
            string passiveHost = data[0] + "." + data[1] + "." + data[2] + "." + data[3];
            // extract and convert the port number from the server response
            int passivePort = Int32.Parse(data[4], CultureInfo.InvariantCulture) * 256 + Int32.Parse(data[5], CultureInfo.InvariantCulture);

            try
            {
                //  create a new tcp client object and use proxy if supplied
                /* if (_proxy != null)
                    _dataConn = _proxy.CreateConnection(passiveHost, passivePort);
                else */
                _dataConn = new TcpClient(passiveHost, passivePort);

				//johnshope.Sync.Log.Debug("Create Passive Connection on Port " + passivePort.ToString());
 
                _dataConn.ReceiveBufferSize = _tcpBufferSize;
                _dataConn.ReceiveTimeout = _tcpTimeout;
                _dataConn.SendBufferSize = _tcpBufferSize;
                _dataConn.SendTimeout = _tcpTimeout;
            }
            catch (Exception ex)
            {
                throw new FtpDataConnectionException(String.Format(CultureInfo.InvariantCulture, "An error occurred while opening passive data connection to destination '{0}' on port '{1}'.", passiveHost, passivePort), ex);
            }
        }

        /// <summary>
        /// Creates an SSL or TLS secured stream.
        /// </summary>
        /// <param name="stream">Unsecured stream.</param>
        /// <returns>Secured stream</returns>
        private Stream CreateSslStream(Stream stream)
        {
            // create an SSL or TLS stream that will close the client's stream
            SslStream ssl = new SslStream(stream, true, new RemoteCertificateValidationCallback(secureStream_ValidateServerCertificate), null);

            // choose the protocol
            SslProtocols protocol = SslProtocols.None;
            switch (_securityProtocol)
            {
                case FtpSecurityProtocol.Tls1OrSsl3Explicit:
                case FtpSecurityProtocol.Tls1OrSsl3Implicit:
                    protocol = SslProtocols.Default;
                    break;
                case FtpSecurityProtocol.Ssl2Explicit:
                case FtpSecurityProtocol.Ssl2Implicit:
                    protocol = SslProtocols.Ssl2;
                    break;
                case FtpSecurityProtocol.Ssl3Explicit:
                case FtpSecurityProtocol.Ssl3Implicit:
                    protocol = SslProtocols.Ssl3;
                    break;
                case FtpSecurityProtocol.Tls1Explicit:
                case FtpSecurityProtocol.Tls1Implicit:
                    protocol = SslProtocols.Tls;
                    break;
                default:
                    throw new FtpSecureConnectionException(String.Format("Unhandled FtpSecurityProtocol type '{0}'.", _securityProtocol.ToString()));
            }

            // The server name must match the name on the server certificate.
            try
            {
                // authenicate the client
                ssl.AuthenticateAsClient(_host, _clientCertificates, protocol, true);
            }
            catch (AuthenticationException authEx)
            {
                throw new FtpAuthenticationException("Secure FTP session certificate authentication failed.", authEx);
            }

            return ssl;
        }

        // the following method is invoked by the RemoteCertificateValidationDelegate.
        private bool secureStream_ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //// if it is the same certificate we have already approved then don't validate it again
            if (_serverCertificate != null && certificate.GetCertHashString() == _serverCertificate.GetCertHashString())
                return true;

            // invoke the ValidateServerCertificate event if the user is subscribing to it
            // ignore our own logic and let the user decide if the certificate is valid or not
            if (ValidateServerCertificate != null)
            {
                ValidateServerCertificateEventArgs args = new ValidateServerCertificateEventArgs(new X509Certificate2(certificate.GetRawCertData()), chain, sslPolicyErrors);
                ValidateServerCertificate(this, args);
                // make a copy of the certificate due to sharing violations
                if (args.IsCertificateValid)
                    _serverCertificate = new X509Certificate2(certificate.GetRawCertData());
                return args.IsCertificateValid;
            }
            else
            {
                // analyze the policy errors and decide if the certificate should be accepted or not.
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
                    throw new FtpCertificateValidationException(String.Format("Certificate validation failed.  The host name '{0}' does not match the name on the security certificate '{1}'.  To override this behavior, subscribe to the ValidateServerCertificate event to validate certificates.", _host, certificate.Issuer));

                if (sslPolicyErrors == SslPolicyErrors.None || (sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    // make a copy of the server certificate due to sharing violations
                    _serverCertificate = new X509Certificate2(certificate.GetRawCertData());
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        
        private void CreateSslExplicitCommandStream()
        {
            try
            {
                // send authentication type request
                string authCommand = "";
                switch (_securityProtocol)
                {
                    case FtpSecurityProtocol.Tls1OrSsl3Explicit:
                    case FtpSecurityProtocol.Ssl3Explicit:
                    case FtpSecurityProtocol.Ssl2Explicit:
                        authCommand = "SSL";
                        break;
                    case FtpSecurityProtocol.Tls1Explicit:
                        authCommand = "TLS";
                        break;
                }

                Debug.Assert(authCommand.Length > 0, "auth command should have a value - make sure every enum option in auth command has a corresponding value");

                SendRequest(new FtpRequest(_encoding, FtpCmd.Auth, authCommand));

                // set the active command stream to the ssl command stream object
                lock (_reponseMonitorLock)
                {
                    _commandStream = CreateSslStream(_commandConn.GetStream());
                }

                SendRequest(new FtpRequest(_encoding, FtpCmd.Pbsz, "0"));
                SendRequest(new FtpRequest(_encoding, FtpCmd.Prot, "P"));
            }
            catch (FtpAuthenticationException fauth)
            {
                throw new FtpSecureConnectionException(String.Format("An ftp authentication exception occurred while setting up a explicit ssl/tls command stream.  {0}", fauth.Message), _response, fauth);                
            }
            catch (FtpException fex)
            {
                throw new FtpSecureConnectionException(String.Format("An error occurred while setting up a explicit ssl/tls command stream.  {0}", fex.Message), _response, fex);
            }

        }

        private void CreateSslImplicitCommandStream()
        {
            try
            {
                // set the active command stream to the ssl command stream object
                lock (_reponseMonitorLock)
                {
                    _commandStream = CreateSslStream(_commandConn.GetStream());
                }
            }
            catch (FtpAuthenticationException fauth)
            {
                throw new FtpSecureConnectionException(String.Format("An ftp authentication exception occurred while setting up a implicit ssl/tls command stream.  {0}", fauth.Message), _response, fauth);
            }
            catch (FtpException fex)
            {
                throw new FtpSecureConnectionException(String.Format("An error occurred while setting up a implicit ssl/tls command stream.  {0}", fex.Message), _response, fex);
            }

        }

        private void DoIntegrityCheck(FtpRequest request, Stream stream, long restartPosition)
        {
            // get the file path from the request argument
            string path = request.Arguments[0];
            long startPos = restartPosition;
            long endPos = stream.Length;

            string streamHash = ComputeChecksum(_hashAlgorithm, stream, startPos);
            string serverHash = GetChecksum(_hashAlgorithm, path, startPos, endPos);

            // string compare the dataHash to the server hash value and see if they are the same
            if (String.Compare(streamHash, serverHash, StringComparison.InvariantCultureIgnoreCase) != 0)
                throw new FtpFileIntegrityException(String.Format("File integrity check failed.  The destination integrity value '{0}' for the file '{1}' did not match the data transfer integrity value '{2}'.", serverHash, path, streamHash));
        }

        #endregion

        #region Destructors

        /// <summary>
        /// Disposes all objects and connections.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose Method.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_asyncWorker != null && _asyncWorker.IsBusy)
                    _asyncWorker.CancelAsync();

                if (_activeListener != null)
                    _activeListener.Stop();

                if (_dataConn != null && _dataConn.Connected)
                    _dataConn.Close();

                if (_commandConn != null && _commandConn.Connected)
                    _commandConn.Close();

                if (_activeSignal != null)
                    _activeSignal.Close();

                if (_responseMonitor != null && _responseMonitor.IsAlive)
                    _responseMonitor.Abort();
            }
        }

        /// <summary>
        /// Dispose deconstructor.
        /// </summary>
        ~FtpBase()
        {
            Dispose(false);
        }

        #endregion
    }
}
