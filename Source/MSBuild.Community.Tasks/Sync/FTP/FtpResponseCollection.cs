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
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Ftp response collection.
    /// </summary>
    public class FtpResponseCollection : IEnumerable<FtpResponse>
    {
        private List<FtpResponse> _list = new List<FtpResponse>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FtpResponseCollection()
        { }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire FtpResponseCollection list.
        /// </summary>
        /// <param name="item">The FtpResponse object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(FtpResponse item)
        {
            return _list.IndexOf(item);
        }



        /// <summary>
        /// Adds an FtpResponse to the end of the FtpResponseCollection list.
        /// </summary>
        /// <param name="item">FtpResponse object to add.</param>
        public void Add(FtpResponse item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the FtpResponseCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<FtpResponse> IEnumerable<FtpResponse>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an FtpResponse from the FtpResponseCollection list based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>FtpResponse object.</returns>
        public FtpResponse this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Remove all elements from the FtpResponseCollection list.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Get the raw FTP server supplied reponse text.
        /// </summary>
        /// <returns>A string containing the FTP server response.</returns>
        public string GetRawText()
        {
            StringBuilder builder = new StringBuilder();
            foreach(FtpResponse item in _list)
            {
                builder.Append(item.RawText);
                builder.Append("\r\n");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get the last server response from the FtpResponseCollection list.
        /// </summary>
        /// <returns>FtpResponse object.</returns>
        public FtpResponse GetLast()
        {
            if (_list.Count == 0)
                return new FtpResponse();
            else
                return _list[_list.Count - 1];
        }
    }
}