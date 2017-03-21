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
using System.Collections.ObjectModel;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Ftp item list.
    /// </summary>
	public class FtpItemCollection : IEnumerable<FtpItem> 
	{
        private List<FtpItem> _list = new List<FtpItem>();

        private long _totalSize;

        private static string COL_NAME = "Name";
        private static string COL_MODIFIED = "Modified";
        private static string COL_SIZE = "Size";
        private static string COL_SYMBOLIC_LINK = "SymbolicLink";
        private static string COL_TYPE = "Type";
        private static string COL_ATTRIBUTES = "Attributes";
        private static string COL_RAW_TEXT = "RawText";

        /// <summary>
        /// Default constructor for FtpItemCollection.
        /// </summary>
        public FtpItemCollection()
        {  }

        /// <summary>
        /// Split a multi-line file list text response and add the parsed items to the collection.
        /// </summary>
        /// <param name="path">Path to the item on the FTP server.</param>
        /// <param name="fileList">The multi-line file list text from the FTP server.</param>
        /// <param name="itemParser">Line item parser object used to parse each line of fileList data.</param>
        public FtpItemCollection(string path, string fileList, IFtpItemParser itemParser)
        {
            Parse(path, fileList, itemParser);
        }

        /// <summary>
        /// Merges two FtpItemCollection together into a single collection.
        /// </summary>
        /// <param name="items">Collection to merge with.</param>
        public void Merge(FtpItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException("items", "must have a value");

            foreach (FtpItem item in items)
            {
                FtpItem newItem = new FtpItem(item.Name, item.Modified, item.Size, item.SymbolicLink, item.Attributes, item.ItemType, item.RawText);
                newItem.ParentPath = item.ParentPath;
                this.Add(newItem);
            }
        }

        private void Parse(string path, string fileList, IFtpItemParser itemParser)
        {
            string[] lines = SplitFileList(fileList);

            int length = lines.Length - 1;
            for (int i = 0; i <= length; i++)
            {
                FtpItem item = itemParser.ParseLine(lines[i]);
				if (item != null && item.Name != "." && item.Name != "..")
                {
                    // set the parent path to the value passed in
                    item.ParentPath = path;
                    _list.Add(item);
                    _totalSize += item.Size;
                }
            }
        }

        private string[] SplitFileList(string response)
        {
            char[] crlfSplit = new char[2];
            crlfSplit[0] = '\r';
            crlfSplit[1] = '\n';
            return response.Split(crlfSplit, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Convert current FtpCollection to a DataTable object.
        /// </summary>
        /// <returns>Data table object.</returns>
        public DataTable ToDataTable()
        {
            DataTable dataTbl = new DataTable();
            dataTbl.Locale = CultureInfo.InvariantCulture;

            CreateColumns(dataTbl);

            foreach (FtpItem item in _list)
            {
                DataRow row = dataTbl.NewRow();
                row[COL_NAME] = item.Name;
                row[COL_MODIFIED] = item.Modified;
                row[COL_SIZE] = item.Size;
                row[COL_SYMBOLIC_LINK] = item.SymbolicLink;
                row[COL_TYPE] = item.ItemType.ToString();
                row[COL_ATTRIBUTES] = item.Attributes;
                row[COL_RAW_TEXT] = item.RawText;
                dataTbl.Rows.Add(row);
            }

            return dataTbl;
        }
        
        private void CreateColumns(DataTable dataTbl)
        {
            dataTbl.Columns.Add(new DataColumn(COL_NAME, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_MODIFIED, typeof(DateTime)));
            dataTbl.Columns.Add(new DataColumn(COL_SIZE, typeof(long)));
            dataTbl.Columns.Add(new DataColumn(COL_TYPE, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_ATTRIBUTES, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_SYMBOLIC_LINK, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_RAW_TEXT, typeof(string)));
        }

        /// <summary>
        /// Gets the size, in bytes, of all files in the collection as reported by the FTP server.
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire FtpItemCollection list.
        /// </summary>
        /// <param name="item">The FtpItem to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(FtpItem item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Adds an FtpItem to the end of the FtpItemCollection list.
        /// </summary>
        /// <param name="item">FtpItem object to add.</param>
        public void Add(FtpItem item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the FtpItemCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<FtpItem> IEnumerable<FtpItem>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an FtpItem from the FtpItemCollection based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>FtpItem</returns>
        public FtpItem this[int index]
        {
            get { return _list[index];  }
        }

        /// <summary>
        /// Searches for the specified object based on the 'name' parameter value
        /// and returns true if an object with the name is found; otherwise false.
        /// </summary>
        /// <param name="name">The name of the FtpItem to locate in the collection.</param>
        /// <returns>True if the name if found; otherwise false.</returns>
        public bool ContainsName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name", "must have a value");

            foreach (FtpItem item in _list)
            {
                if (name == item.Name)
                    return true;
            }

            return false;
        }

		public void SetTimeOffset(int offset) {
			foreach (var item in this) item.SetTimeOffset(offset);
		}

    }
} 