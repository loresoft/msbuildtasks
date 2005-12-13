#region Copyright © 2005 Peter G Jones. All rights reserved.
/*
Copyright © 2005 Peter G Jones. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Data.SqlClient;

// $Id$

namespace MSBuild.Community.Tasks
{
    public class SqlExecute : Task
    {
        public override bool Execute()
        {
            SqlConnection con = null;
            SqlCommand cmd = null;
            _result = -1;

            try
            {
                con = new SqlConnection(ConnectionString);
                cmd = new SqlCommand(Command, con);
                con.Open();

                _result = cmd.ExecuteNonQuery();

                Log.LogMessage("Successfully executed SQL command with result = : " + _result.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError("Error executing SQL command: {0}\n{1}", Command, ex.Message);
                return false;
            }
            finally
            {
                if (con != null)
                    con.Close();
            }
        }

        #region private decls
        private string _conStr;
        private string _cmd;
        private int _result;
        #endregion

        /// <summary>
        /// The connection string
        /// </summary>
        [Required]
        public string ConnectionString
        {
            get { return _conStr; }
            set { _conStr = value; }
        }

        /// <summary>
        /// The command to execute
        /// </summary>
        [Required]
        public string Command
        {
            get { return _cmd; }
            set { _cmd = value; }
        }

        /// <summary>
        /// Output the return count/value
        /// </summary>
        [Output]
        public int Result
        {
            get
            {
                return _result;
            }
        }
    }
}
