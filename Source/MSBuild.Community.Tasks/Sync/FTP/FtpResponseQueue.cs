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
using System.Collections.Generic;
using System.Text;


namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Thread safe FtpResponse queue object.
    /// </summary>
    internal class FtpResponseQueue
    {
        private Queue<FtpResponse> _queue = new Queue<FtpResponse>(10);
        
        /// <summary>
        /// Gets the number of elements contained in the FtpResponseQueue.
        /// </summary>
        public int Count 
        {
            get 
            {
                lock (this) 
                { 
                    return _queue.Count; 
                } 
            } 
        }





        /// <summary>
        /// Adds an Response object to the end of the FtpResponseQueue.
        /// </summary>
        /// <param name="item">An FtpResponse item.</param>
        public void Enqueue(FtpResponse item)
        {
            lock (this)
            {
                _queue.Enqueue(item);
            }
        }

        /// <summary>
        /// Removes and returns the FtpResponse object at the beginning of the FtpResponseQueue.
        /// </summary>
        /// <returns>FtpResponse object at the beginning of the FtpResponseQueue</returns>
        public FtpResponse Dequeue()
        {
            lock (this)
            {
                return _queue.Dequeue();
            }
        }





    }
}
