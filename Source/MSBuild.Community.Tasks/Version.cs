#region Copyright © 2005 Paul Welter. All rights reserved.
/*
Copyright © 2005 Paul Welter. All rights reserved.

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
using System.IO;

// $Id$

namespace MSBuild.Community.Tasks
{
	/// <summary>
	/// Get Version information from file.
	/// </summary>
	/// <example>Get version information and increment revision.
	/// <code><![CDATA[
	/// <Version VersionFile="number.txt" BuildType="Automatic" RevisionType="Increment">
	///     <Output TaskParameter="Major" PropertyName="Major" />
	///     <Output TaskParameter="Minor" PropertyName="Minor" />
	///     <Output TaskParameter="Build" PropertyName="Build" />
	///     <Output TaskParameter="Revision" PropertyName="Revision" />
	/// </Version>
	/// <Message Text="Version: $(Major).$(Minor).$(Build).$(Revision)"/>
	/// ]]></code>
	/// </example>
	public class Version : Task {

        #region Enumerators
        private enum BuildTypeEnum {
            None,
            Automatic,
            Increment,
            Date,
            DateIncrement
        }
        private enum RevisionTypeEnum {
            None,
            Automatic,
            Increment,
            NonIncrement
        }
        #endregion

        #region Constructor
        /// <summary>
		/// Initializes a new instance of the <see cref="T:Version"/> class.
		/// </summary>
		public Version()
		{
		}

		#endregion Constructor

        private System.Version _originalValues;

		#region Output Parameters
		private int _major = 1;

		/// <summary>
		/// Gets or sets the major version number.
		/// </summary>
		/// <value>The major version number.</value>
		[Output]
		public int Major
		{
			get { return _major; }
			set { _major = value; }
		}

		private int _minor;

		/// <summary>
		/// Gets or sets the minor version number.
		/// </summary>
		/// <value>The minor version number.</value>
		[Output]
		public int Minor
		{
			get { return _minor; }
			set { _minor = value; }
		}

		private int _build;

		/// <summary>
		/// Gets or sets the build version number.
		/// </summary>
		/// <value>The build version number.</value>
		[Output]
		public int Build
		{
			get { return _build; }
			set { _build = value; }
		}

		private int _revision;

		/// <summary>
		/// Gets or sets the revision version number.
		/// </summary>
		/// <value>The revision version number.</value>
		[Output]
		public int Revision
		{
			get { return _revision; }
			set { _revision = value; }
		}

		#endregion Output Parameters

		#region Input Parameters
		private string _versionFile;

		/// <summary>
		/// Gets or sets the version file.
		/// </summary>
		/// <value>The version file.</value>
		[Required]
		public string VersionFile
		{
			get { return _versionFile; }
			set { _versionFile = value; }
		}

        private BuildTypeEnum _buildTypeEnum;

		/// <summary>
		/// Gets or sets the type of the build. Possible values are None, Automatic, Increment, Date, DateIncrement
		/// </summary>
		/// <remarks>
		/// Possible values include None, Automatic, Increment, NonIncrement, or DateIncrement.  If value is not provided, None is assumed.
        /// If the BuildType of DateIncrement is selected, the StartAt parameter will be used to calculate the build number based on the the number of days that passed
        /// between the StartDate and the current date.
		/// </remarks>
		public string BuildType
		{
			get { return _buildTypeEnum.ToString(); }
			set {_buildTypeEnum = ((value == string.Empty) || (value == null)) ? BuildTypeEnum.None : (BuildTypeEnum)Enum.Parse(typeof(BuildTypeEnum), value);}
		}

        private RevisionTypeEnum _revisionTypeEnum;

		/// <summary>
        /// Gets or sets the type of the revision. Possible values are None, Automatic, Increment, NonIncrement.
		/// </summary>
		/// <remarks>
		/// Possible values include None, Automatic, Increment, NonIncrement.
		/// </remarks>
		public string RevisionType
		{
			get { return _revisionTypeEnum.ToString(); }
			set { _revisionTypeEnum = ((value == string.Empty) || (value == null)) ? RevisionTypeEnum.None : (RevisionTypeEnum)Enum.Parse(typeof(RevisionTypeEnum), value);}
		}

        private DateTime _startDate = new DateTime(2000, 1, 1);
        /// <summary>
        /// Gets or sets the starting date used to calculate the revision.
        /// </summary>
        /// <value>The starting date for calculation of the revision.</value>
        /// <remarks>
        /// This value is used in conjunction with the BuildType of Date. <seealso cref="BuildType"/> This parameter defaults to 2000-01-01.
        /// </remarks>
        public string StartDate {
            get { return _startDate.ToString(); }
            set { _startDate = DateTime.Parse(value); }
        }

		#endregion Input Parameters

		#region Task Overrides
		/// <summary>
		/// When overridden in a derived class, executes the task.
		/// </summary>
		/// <returns>
		/// true if the task successfully executed; otherwise, false.
		/// </returns>
		public override bool Execute()
		{
			ReadVersionFromFile();
			CalculateBuildNumber();
			CalculateRevisionNumber();
			return WriteVersionToFile();
		}

		#endregion Task Overrides

		#region Private Methods
		private void ReadVersionFromFile()
		{
			string textVersion = null;
			System.Version version = null;

			if (!System.IO.File.Exists(_versionFile))
			{
				Log.LogWarning(Properties.Resources.VersionFileNotFound, _versionFile);
                _originalValues = new System.Version(_major, _minor, _build, _revision);
				return;
			}

			try
			{
				using (StreamReader reader = new StreamReader(_versionFile))
				{
					textVersion = reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				Log.LogError(Properties.Resources.VersionReadException,
					_versionFile, ex.Message);
				return;
			}

			Log.LogMessage(Properties.Resources.VersionRead,
				textVersion, _versionFile);

			try
			{
				version = new System.Version(textVersion);
			}
			catch (Exception ex)
			{
				Log.LogErrorFromException(ex);
				return;
			}

			if (version != null)
			{
				_major = version.Major;
				_minor = version.Minor;
				_build = version.Build;
				_revision = version.Revision;
                _originalValues = version;
			}
		}

		private bool WriteVersionToFile()
		{
			System.Version version = new System.Version(_major, _minor, _build, _revision);

			try
			{
				using (StreamWriter writer = System.IO.File.CreateText(_versionFile))
				{
					writer.Write(version.ToString());
					writer.Flush();
				}
			}
			catch (Exception ex)
			{
				Log.LogError(Properties.Resources.VersionWriteException,
					_versionFile, ex.Message);
				return false;
			}

			Log.LogMessage(Properties.Resources.VersionWrote,
				version.ToString(), _versionFile);

			return true;
		}

		private int CalculateDaysSinceMilenium()
		{
			DateTime today = DateTime.Now;
			DateTime startDate = new DateTime(2000, 1, 1);
			TimeSpan span = today.Subtract(startDate);
			return (int)span.TotalDays;
		}

		private int CalculateBuildDate()
		{
			DateTime dDate = DateTime.Now;
			int _month = dDate.Month * 100;
			int _day = dDate.Day;
			int _year = (dDate.Year % 2000) * 10000;

			return (_year + _month + _day);
		}

		private void CalculateBuildNumber()
		{

            switch (_buildTypeEnum) 
            {
                case BuildTypeEnum.Automatic:
                    _build = CalculateDaysSinceMilenium();
                    break;
                case BuildTypeEnum.Increment:
                    _build++;
                    break;
                case BuildTypeEnum.Date:
                    _build = CalculateBuildDate();
                    break;
                case BuildTypeEnum.DateIncrement:
                    _build = CalculateBuildDateIncrememt();
                    break;
                default:
                    break;
            }
		}

        private int CalculateBuildDateIncrememt() {
            TimeSpan diff = DateTime.Now.Subtract(_startDate);
            return diff.Days;

        }

		private void CalculateRevisionNumber()
		{
            switch (_revisionTypeEnum) {
                case RevisionTypeEnum.Automatic:
                    _revision = (int)DateTime.Now.TimeOfDay.TotalSeconds;
                    break;
                case RevisionTypeEnum.Increment:
                    if (_buildTypeEnum == BuildTypeEnum.DateIncrement) _revision = CalculateBuildDateIncrememtRevision(); else _revision++;
                    break;
                case RevisionTypeEnum.NonIncrement:
                    break;
                case RevisionTypeEnum.None:
                    break;
                default:
                    break;
            }
		}

        private int CalculateBuildDateIncrememtRevision() 
        {
            if (_build != _originalValues.Build && _originalValues.Revision > 0) return 0; else return _revision + 1;
        }
		#endregion Private Methods

	}
}
