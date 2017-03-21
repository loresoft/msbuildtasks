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
using System.Runtime.Serialization;

namespace Starksoft.Net.Ftp
{

    /// <summary>
    /// This exception is thrown when a general FTP exception occurs.
    /// </summary>
    [Serializable()]
    public class FtpException : Exception
    {
        private FtpResponse _response = new FtpResponse();
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public FtpException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public FtpException(string message)
            : base(message)
        {
        }
 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpException(string message, Exception innerException)
            :
           base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Response object.</param>
        /// <param name="innerException">The inner exception object.</param>
        public FtpException(string message, FtpResponse response, Exception innerException)
            :
           base(message, innerException)
        {
            _response = response;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        /// <param name="response">Ftp response object.</param>
        public FtpException(string message, FtpResponse response)
            : base(message)
        {
            _response = response;
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Stream context information.</param>
        protected FtpException(SerializationInfo info,
           StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the last FTP response if one is available.
        /// </summary>
        public FtpResponse LastResponse
        {
            get { return _response; }
        }


        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get
            {
                if (_response.Code == FtpResponseCode.None)
                    return base.Message;
                else
                    return String.Format("{0}  (Last Server Response: {1}  {2})", base.Message, _response.Text, _response.Code); ;
            }
        }
    }

}
