// $Id$
// Copyright © 2006 Ignaz Kohlbecker

using System;
using System.Globalization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// A task to get a current time stamp for use in a property.
    /// </summary>
    /// <remarks>
    /// See
    /// <a href="ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref8/html/T_System_Globalization_DateTimeFormatInfo.htm">
    /// DateTimeFormatInfo</a>
    /// for the syntax of the format string.
    /// </remarks>
    /// <example>Set property "BuildDate" to the current date and time.
    /// <code><![CDATA[
    /// <Timestamp Format="yyyyMMddHHmmss">
    ///     <Output TaskParameter="LocalTimestamp" PropertyName="buildDate" />
    /// </Timestamp>]]></code>
    /// </example>
    public class Timestamp : Task
    {
        #region Fields

        private string format = null;
        private DateTime timestamp;
        private string localTimestamp = null;
        private string utcTimestamp = null;

        #endregion Fields

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TimeStamp"/> class.
        /// </summary>
        public Timestamp()
        {
        }

        #endregion Constructor

        #region Input Parameters
        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The type of the revision.</value>
        /// <remarks>
        /// See
        /// <a href="ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref8/html/T_System_Globalization_DateTimeFormatInfo.htm">
        /// DateTimeFormatInfo</a>
        /// for the syntax of the format string.
        /// </remarks>
        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        #endregion Input Parameters

        #region Output Parameters
        /// <summary>
        /// Gets the formatted time stamp, in local time.
        /// </summary>
        /// <value>The time stamp as a string, formatted as specified in <see cref="Format"/>.</value>
        [Output]
        public string LocalTimestamp
        {
            get { return localTimestamp; }
        }

        /// <summary>
        /// Gets the formatted time stamp, in universal coordinated time.
        /// </summary>
        /// <value>The time stamp as a string, formatted as specified in <see cref="Format"/>.</value>
        [Output]
        public string UniversalTimestamp
        {
            get { return utcTimestamp; }
        }

        #endregion Output Parameters

        #region Public Properties

        /// <summary>
        /// Gets the time stamp value, in local time.
        /// </summary>
        public DateTime LocalTimestampValue
        {
            get { return timestamp; }
        }

        #endregion Public Properties

        #region Task overrides
        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the task successfully executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool Execute()
        {
            timestamp = DateTime.Now;

            try
            {
                if (format == null)
                {
                    localTimestamp = timestamp.ToLocalTime().ToString(DateTimeFormatInfo.InvariantInfo);
                    utcTimestamp = timestamp.ToUniversalTime().ToString(DateTimeFormatInfo.InvariantInfo);
                }
                else
                {
                    localTimestamp = timestamp.ToLocalTime().ToString(format, DateTimeFormatInfo.InvariantInfo);
                    utcTimestamp = timestamp.ToUniversalTime().ToString(format, DateTimeFormatInfo.InvariantInfo);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Task overrides
    }
}
