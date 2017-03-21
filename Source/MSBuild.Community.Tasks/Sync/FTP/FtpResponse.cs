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

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// FTP response class containing the FTP raw text, response code, and other information.
    /// </summary>
    public class FtpResponse
    {
        private string _rawText;
        private string _text;
        private FtpResponseCode _code = FtpResponseCode.None;
        private bool _isInformational;

        /// <summary>
        /// Default constructor for FtpResponse.
        /// </summary>
        public FtpResponse()
        { }

        /// <summary>
        /// Constructor for FtpResponse.
        /// </summary>
        /// <param name="rawText">Raw text information sent from the FTP server.</param>
        public FtpResponse(string rawText)
        {
            _rawText = rawText;
            _text = ParseText(rawText);
            _code = ParseCode(rawText);
            _isInformational = ParseInformational(rawText);
        }

        /// <summary>
        /// Constructor for FtpResponse.
        /// </summary>
        /// <param name="response">FtpResponse object.</param>
        public FtpResponse(FtpResponse response)
        {
            _rawText = response.RawText;
            _text = response.Text;
            _code = response.Code;
            _isInformational = response.IsInformational;
        }

        /// <summary>
        /// Get raw server response text information.
        /// </summary>
        public string RawText
        {
            get { return _rawText; }
        }

        /// <summary>
        /// Get the server response text.
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Get a value indicating the FTP server response code.
        /// </summary>
        public FtpResponseCode Code
        {
            get { return _code; }
        }

        internal bool IsInformational
        {
            get { return _isInformational; }
        }

        private FtpResponseCode ParseCode(string rawText)
        {
            FtpResponseCode code = FtpResponseCode.None;

            if (rawText.Length >= 3)
            {
                string codeString = rawText.Substring(0, 3);
                int codeInt = 0;

                if (Int32.TryParse(codeString, out codeInt))
                {
                    code = (FtpResponseCode)codeInt;
                }
            }

            return code;
        }

        private string ParseText(string rawText)
        {
            if (rawText.Length > 4)
                return rawText.Substring(4).Trim();
            else
                return string.Empty;
        }

        private bool ParseInformational(string rawText)
        {
            if (rawText.Length >= 4 && rawText[3] == '-')
                return true;
            else
                return false;
        }


    }
}
