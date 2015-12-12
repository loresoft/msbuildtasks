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
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Event arguments to facilitate the FtpClient transfer progress and complete events.
    /// </summary>
    public class ValidateServerCertificateEventArgs : EventArgs
    {

        private X509Certificate2 _certificate;
        private X509Chain _chain;
        private SslPolicyErrors _policyErrors;
        private bool _isCertificateValid;

        /// <summary>
        /// ValidateServerCertificateEventArgs constructor.
        /// </summary>
        /// <param name="certificate">X.509 certificate object.</param>
        /// <param name="chain">X.509 certificate chain.</param>
        /// <param name="policyErrors">SSL policy errors.</param>
        public ValidateServerCertificateEventArgs(X509Certificate2 certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            _certificate = certificate;
            _chain = chain;
            _policyErrors = policyErrors;
        }

        /// <summary>
        /// The X.509 version 3 server certificate.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get { return _certificate; }
        }

        /// <summary>
        /// Server chain building engine for server certificate.
        /// </summary>
        public X509Chain Chain
        {
            get { return _chain; }
        }

        /// <summary>
        /// Enumeration representing SSL (Secure Socket Layer) errors.
        /// </summary>
        public SslPolicyErrors PolicyErrors
        {
            get { return _policyErrors; }
        }

        /// <summary>
        /// Boolean value indicating if the server certificate is valid and can
        /// be accepted by the FtpClient object.
        /// </summary>
        /// <remarks>
        /// Default value is false which results in certificate being rejected and the SSL
        /// connection abandoned.  Set this value to true to accept the server certificate 
        /// otherwise the SSL connection will be closed.
        /// </remarks>
        public bool IsCertificateValid
        {
            get { return _isCertificateValid; }
            set { _isCertificateValid = value; }
        }
    }
}
