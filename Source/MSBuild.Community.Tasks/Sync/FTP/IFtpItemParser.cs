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
    /// <para>
    /// This interface is used to create a pluggable, custom ftp item parser.  The FtpClient object has a property named
    /// ItemParser which is used to override the default item parser behavior.  You might need to create a custom parser for
    /// exotic FTP servers which the FtpClient object does not support.  There is no standard supported in the RFC 959 standard
    /// as to what format an FTP server must give for directory and file listings.  Although newer FTP protocol standards so
    /// support a structured directory listing with detailed information, this new format is not widely supported amoung FTP 
    /// server vendors and there is no hope for support for legacy FTP servers.  
    /// </para>
    /// <para>
    /// The FtpClient object can handle the most common formats without issue but for some older or more exotic FTP servers you may 
    /// such as an MVS legacy system may use a very different format than the common Unix and DOS style format. 
    /// In this situation, it makes the most sense to create your own ftp item parser to parse the unique directory and file listing.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When creating a custom ftp item parser for a specific FTP server you must create a new object and implment the ParseLine method.
    /// Each line of directory listing data that is transmitted from the FTP server will result in the PareLine method being called.  You must
    /// parse the line of data and return a FtpItem object so that the item is added to the FtpItemList collection within the FtpClient object.
    /// </para>
    /// <para>
    /// Below is an example of a custom FtpItem parser that handles both DOS and OS/2 style FTP server listings.  Note that not all the information
    /// such as file permissions can be obtained from the FTP server.  
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    ///using System;
    ///using System.Text.RegularExpressions; 
    ///
    ///public class CustomFtpItemParser : IFtpItemParser  
    ///{
    ///    public FtpItem ParseLine(string line)
    ///    {
    ///        //  date portion
    ///        Regex rdate = new System.Text.RegularExpressions.Regex(@"(\d\d-\d\d-\d\d)");
    ///        string date = rdate.Match(line).ToString();
    ///
    ///        // time portion
    ///        Regex rtime = new System.Text.RegularExpressions.Regex(@"(\d\d:\d\d\s*(AM|PM))|(\d\d:\d\d)", RegexOptions.IgnoreCase);
    ///        string time = rtime.Match(line).ToString();
    ///
    ///        // file size portion
    ///        Regex rsize = new System.Text.RegularExpressions.Regex(@"((?&lt;=(\d\d:\d\d\s*(AM|PM)\s*))\d+)|(\d+(?=\s+\d\d-\d\d-\d\d\s+))", RegexOptions.IgnoreCase);
    ///        string size = rsize.Match(line).ToString();
    ///
    ///        // directory
    ///        Regex rdir = new System.Text.RegularExpressions.Regex(@"&lt;DIR&gt;|\sDIR\s", RegexOptions.IgnoreCase);
    ///        string dir = rdir.Match(line).ToString();
    ///
    ///        // name
    ///        Regex rname = new System.Text.RegularExpressions.Regex(@"((?&lt;=&lt;DIR&gt;\s+).+)|((?&lt;=\d\d:\d\d\s+).+)|((?&lt;=(\d\d:\d\d(AM|PM)\s+\d+\s+)).+)", RegexOptions.IgnoreCase);
    ///        string name = rname.Match(line).ToString();
    ///
    ///        // put togther the date/time
    ///        DateTime dateTime = DateTime.MinValue;
    ///        DateTime.TryParse(String.Format("{0} {1}", date, time), out dateTime);
    ///
    ///        // parse the file size
    ///        long sizeLng = 0;
    ///        Int64.TryParse(size, out sizeLng);           
    ///
    ///        // determine the file item itemType
    ///        FtpItemType itemTypeObj;
    ///        if (dir.Length &gt; 0)
    ///            itemTypeObj = FtpItemType.Directory;
    ///        else
    ///            itemTypeObj = FtpItemType.File;
    ///
    ///        return new FtpItem(name, dateTime, sizeLng, "", "", itemTypeObj, line);
    ///    }
    ///}       
    /// </code>
    ///
    /// <code>
    /// // create a FtpClient object to some local windows ftp server in your organization
    /// FtpClient ftp = new FtpClient("192.168.1.1");
    ///
    /// // use the custom ftp item parser
    /// ftp.ItemParser = new CustomFtpItemParser();
    ///
    /// // open a connect to the rserver
    /// ftp.Open("ftp", "user@mail.com");
    /// FtpItemList list = ftp.GetDirList("/");
    ///
    /// // list all the items to the debug output window
    /// foreach (FtpItem item in list)
    /// {
    ///     System.Diagnostics.Debug.WriteLine(item.Name + " " + item.Modified.ToString() + " " + item.Size.ToString() + " " 
    ///         + item.SymbolicLink + " " + item.Type.ToString() + " " + item.Permissions + " ---- " + item.RawText);   
    /// }
    /// 
    /// ftp.Close();
    /// </code>
    /// </example>
    
    public interface IFtpItemParser
    {
        /// <summary>
        /// The ParseLine method is called by the FtpClient for each line of directory listing data transmitted by the FTP server to the FTP client.
        /// </summary>
        /// <param name="line">A single line of data for a specific directory for file listing.</param>
        /// <returns>A new FtpItem object.</returns>
       
        FtpItem ParseLine(string line);
    }
}



