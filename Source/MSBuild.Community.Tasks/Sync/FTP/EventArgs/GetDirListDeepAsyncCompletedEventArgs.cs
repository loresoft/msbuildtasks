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
using System.ComponentModel;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Provides data for the GetDirAsyncCompleted event.
    /// </summary>
    public class GetDirListDeepAsyncCompletedEventArgs : AsyncCompletedEventArgs
    {
        private FtpItemCollection _directoryListing;

        /// <summary>
        ///  Initializes a new instance of the GetDirAsyncCompletedEventArgs class.
        /// </summary>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="canceled">A value indicating whether the asynchronous operation was canceled.</param>
        /// <param name="directoryListing">A FtpItemCollection containing the directory listing.</param>
        public GetDirListDeepAsyncCompletedEventArgs(Exception error, bool canceled, FtpItemCollection directoryListing)
            : base(error, canceled, null)
        {
            _directoryListing = directoryListing;
        }

        /// <summary>
        /// Directory listing collection.
        /// </summary>
        public FtpItemCollection DirectoryListingResult
        {
            get
            {
                //base.RaiseExceptionIfNecessary();
                return _directoryListing;
            }
        }
    }

}